using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Email;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Security.Claims;
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
        private readonly IManequimService _manequim;
        private readonly IMapper _mapper;

        public IdentityController(
            SignInManager<UsuarioIdentity>  signInManager,
            UserManager<UsuarioIdentity> userManager,
            IUserStore<UsuarioIdentity> userStore,
            RoleManager<IdentityRole> roleManager,
            IPessoaService pessoaService,
            IManequimService manequim,
            IMapper mapper
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;

            _pessoaService = pessoaService;
            _manequim = manequim;
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
                else
                {
                   
                        Notificar("<span class=\"fw-bold fs-5 mt-3\">Erro ! Houve um erro no login, não é possível realizar a autenticação.",
               Notifica.Erro);
                 
                }
            }
            return View("Autenticar");
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
                user.Email = model.Pessoa.Email;
                await _userStore.SetUserNameAsync(user, model.Pessoa.Cpf, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Senha);

                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR SISTEMA");
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR SISTEMA"));
                    }

                    var userDb = await _userManager.FindByNameAsync(model.Pessoa.Cpf);
                    await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR SISTEMA");

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
                RequestPasswordReset(_userManager, model.Email, await _pessoaService.GetNomeAssociadoByEmail(model.Email));
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
                //ativa associado
                HttpStatusCode atv = await _pessoaService.AtivarAssociado(user.UserName);

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

        [HttpGet]
        [Authorize]
        public ActionResult Perfil()
        {
            var papelGrupo = User.FindFirst("IdPapelGrupo")?.Value ?? "4";

            ViewData["Layout"] = "_LayoutColaborador";

            switch (papelGrupo)
            {
                case "1":
                    ViewData["Layout"] = "_LayoutAssociado";
                break;
            }

            var user = _pessoaService.Get(Convert.ToInt32(User.FindFirst("Id")?.Value));
            if(user == null)
            {
                return RedirectToAction(nameof(Autenticar), "Identity");
            }
           
           var userModel = _mapper.Map<UserViewModel>(user);
           userModel.ListaManequim = new SelectList(_manequim.GetAll(), "Id", "Tamanho", userModel.IdManequim);
            return View(userModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Perfil(UserViewModel userInfos)
        {
            if(ModelState.IsValid)
            {
                var pessoaModel = _mapper.Map<Pessoa>(userInfos);
                pessoaModel.IdGrupoMusical = Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value);
                pessoaModel.IdPapelGrupo = Convert.ToInt32(User.FindFirst("IdPapelGrupo")?.Value);
                pessoaModel.Cpf = User.Identity?.Name ?? "";

                switch(await _pessoaService.UpdateUserInfos(pessoaModel, userInfos.CurrentPassword, userInfos.Password))
                {
                    case HttpStatusCode.OK:
                        Notificar("Informações <b>Salvas</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                        await UpdateClaims("UserName", userInfos.Nome?.Split(" ")[0]);
                    break;
                    case HttpStatusCode.BadRequest:
                        Notificar("Ocorreu um <b>Erro</b> durante a <b>Atualização</b> das <b>Informações</b>", Notifica.Erro);
                    break;
                    case HttpStatusCode.NotFound:
                        Notificar("Ocorreu um <b>Erro</b> durante o <b>Acesso</b> as <b>Informações</b>", Notifica.Erro);
                    break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("Ocorreu um <b>Erro Interno</b> durante a atualização das <b>Informações</b>", Notifica.Erro);
                    break;
                }
            }

            ViewData["Layout"] = "_LayoutColaborador";
            var papelGrupo = User.FindFirst("IdPapelGrupo")?.Value ?? "4";
            switch (papelGrupo)
            {
                case "1":
                    ViewData["Layout"] = "_LayoutAssociado";
                break;
            }

            userInfos.ListaManequim = new SelectList(_manequim.GetAll(), "Id", "Tamanho", userInfos.IdManequim);

            return View(userInfos);
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult PerfilSistema()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PerfilSistema(UserSystemViewModel userInfos)
        {
            if(ModelState.IsValid)
            {
                switch(await _pessoaService.UpdateUAdmSistema(User.Identity?.Name, userInfos.CurrentPassword, userInfos.Password))
                {
                    case HttpStatusCode.OK:
                        Notificar("Informações <b>Salvas</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        return View();
                    case HttpStatusCode.BadRequest:
                        Notificar("Ocorreu um <b>Erro</b> durante a <b>Atualização</b> das <b>Informações</b>", Notifica.Erro);
                    break;
                    case HttpStatusCode.NotFound:
                        Notificar("Ocorreu um <b>Erro</b> durante o <b>Acesso</b> as <b>Informações</b>", Notifica.Erro);
                    break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("Ocorreu um <b>Erro Interno</b> durante a atualização das <b>Informações</b>", Notifica.Erro);
                    break;
                }
            }
            return View(userInfos);
        }

        private async Task UpdateClaims(string claimName, string? claimValue)
        {
            var Identity = HttpContext.User.Identity as ClaimsIdentity;
            Identity?.RemoveClaim(Identity.FindFirst(claimName));
            Identity?.AddClaim(new Claim(claimName, claimValue ?? ""));

            await _signInManager.RefreshSignInAsync(await _userManager.FindByNameAsync(User.Identity?.Name));
        }
    }
}
