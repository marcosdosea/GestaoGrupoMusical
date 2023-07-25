using AutoMapper;
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
            await _signInManager.SignOutAsync();
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
                RequestPasswordReset(_userManager, model.Email, await _pessoaService.GetNomeAssociado(User.Identity.Name));
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
                return RedirectToAction("PasswordChanged", new { change = false });
            }

            var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);

            //caso usuario nao seja encontrado
            if (user == null)
            {
                // TODO
                // criar uma notificação dizendo que ocorreu um erro ao tentar resetar senha
                // NÃO DIZER QUAL FOI O ERRO OU O MOTIVO
                return RedirectToAction("PasswordChanged", new { change = false });
            }

            var result = await _userManager.ResetPasswordAsync(user,resetPasswordModel.Code, resetPasswordModel.Password);

            //caso haja sucesso em redefinir senha
            if (result.Succeeded)
            {
                // TODO
                // apresentar uma notificação de senha redefinida com sucesso
                return RedirectToAction("PasswordChanged", new { change = true });
            }
            else 
            {
                // TODO
                // criar uma notificação dizendo que ocorreu um erro ao tentar resetar senha
                // NÃO DIZER QUAL FOI O ERRO OU O MOTIVO
                return RedirectToAction("PasswordChanged", new { change = false });
            }
        }

        public IActionResult PasswordChanged(bool change) {
            if(change)
            {
                Notificar("<b>Senha alterada com sucesso!</b>", Notifica.Sucesso);
            }
            else
            {
                Notificar("<b>Erro</b> ao tentar alterar senha!", Notifica.Erro);
            }
            return View();
        }
    }
}
