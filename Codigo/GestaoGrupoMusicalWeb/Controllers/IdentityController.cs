﻿using AutoMapper;
using Core;
using Core.Service;
using Email;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static GestaoGrupoMusicalWeb.Models.IdentityViewModel;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class IdentityController : BaseController
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IUserStore<UsuarioIdentity> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public IdentityController(
            SignInManager<UsuarioIdentity>  signInManager,
            UserManager<UsuarioIdentity> userManager,
            IUserStore<UsuarioIdentity> userStore,
            RoleManager<IdentityRole> roleManager,
            IPessoaService pessoaService,
            IMapper mapper
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;

            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Autenticar()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Autenticar(AutenticarViewModel model)
        {
            model.Cpf = model.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Cpf, model.Senha, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            CadastrarViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastrarViewModel model)
        {
            model.Pessoa.IdPapelGrupo = 3;
            model.Pessoa.IdGrupoMusical = 1;
            model.Pessoa.IdManequim = 1;
            model.Pessoa.Cpf = model.Pessoa.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
            model.Pessoa.Cep = model.Pessoa.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Pessoa.Cpf, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Senha);

                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                    }

                    var userDb = await _userManager.FindByNameAsync(model.Pessoa.Cpf);
                    await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR GRUPO");

                    await _pessoaService.Create(_mapper.Map<Pessoa>(model.Pessoa));

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index","Home");    
                }
                
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sair()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Autenticar");
        }

        private UsuarioIdentity CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UsuarioIdentity>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UsuarioIdentity)}'. " +
                    $"Ensure that '{nameof(UsuarioIdentity)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
        /// <summary>
        /// Metodo para esquecer senha
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Metodo post para solicitar resetar password
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken] 
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Notificar("<span class=\"fw-bold fs-5 mt-3\">Se o email estiver " +
                "cadastrado será enviado um link para redefinir sua senha!</span>",
                Notifica.Sucesso);

            if (ModelState.IsValid)
            { 
                var user = await _userManager.FindByEmailAsync(model.Email);

                //a segunda condição é para caso seja necessario
                //confirmar o email do usuario
                if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
                {
                    return View();
                }

                //gera o token para redefinir senha
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                //gera link para a view da controladora ja passando codigo e id do usuario
                var callbackUrl = Url.Action("ResetPassword", "Identity", new { userId = user.Id, token = code }, /*protocol:*/ Request.Scheme);

                //enviar email com o link
                EmailModel email = new()
                {
                    Assunto = "Batalá - Redefinição de Senha",
                    Body = "<div style=\"text-align: center;\">\r\n    " +
                    "<h1>Redefinição de Senha</h1>\r\n    " +
                    $"<h2>Olá, aqui está o link para redefinir sua senha:</h2>\r\n" +
                    $"<a href=\"{callbackUrl}\" style=\"font-weight: 600;\">Clique Aqui</a>"
                };

                email.To.Add(model.Email);

                await EmailService.Enviar(email);

            }
            
            return View();
        }

        //[AllowAnonymous]
        public ActionResult ResetPassword(string userId, string token)
        {
            ResetPasswordViewModel resetPasswordModel = new();

            resetPasswordModel.UserId = userId;
            resetPasswordModel.Code = token;

            return View(resetPasswordModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordModel)
        {

            if (!ModelState.IsValid || resetPasswordModel.Code == null || resetPasswordModel.UserId == null)
            {
                Notificar("<b>Erro</b> ao tentar alterar senha!", Notifica.Erro);
                return View(resetPasswordModel);
            }

            var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);

            //caso usuario nao seja encontrado
            if (user == null)
            {
                // TODO
                // criar uma notificação dizendo que ocorreu um erro ao tentar resetar senha
                // NÃO DIZER QUAL FOI O ERRO OU O MOTIVO
                Notificar("<b>Erro</b> ao tentar alterar senha!", Notifica.Erro);
                return View(resetPasswordModel);
            }

            var result = await _userManager.ResetPasswordAsync(user,resetPasswordModel.Code, resetPasswordModel.Password);

            //caso haja sucesso em redefinir senha
            if (result.Succeeded)
            {
                // TODO
                // apresentar uma notificação de senha redefinida com sucesso
                Notificar("<b>Senha alterada com sucesso!</b>", Notifica.Sucesso);
                return View();
            }
            else 
            {
                // TODO
                // criar uma notificação dizendo que ocorreu um erro ao tentar resetar senha
                // NÃO DIZER QUAL FOI O ERRO OU O MOTIVO
                Notificar("<b>Erro</b> ao tentar alterar senha!", Notifica.Erro);
                return View(resetPasswordModel);
            }
        }
    }
}
