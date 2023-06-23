using Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static GestaoGrupoMusicalWeb.Models.IdentityViewModel;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class IdentityController : Controller
    {
        private readonly SignInManager<UsuarioIdentity> _signInManager;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IUserStore<UsuarioIdentity> _userStore;

        public IdentityController(
            SignInManager<UsuarioIdentity>  signInManager,
            UserManager<UsuarioIdentity> userManager,
            IUserStore<UsuarioIdentity> userStore
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
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
            //TODO: Cadastrar e associar a atribuição de papel, grupo e manequim ao Adm do Grupo
            model.Pessoa.IdPapelGrupo = 3;
            model.Pessoa.IdGrupoMusical = 1;
            model.Pessoa.IdManequim = 1;
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Pessoa.Cpf, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Senha);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "ADMINISTRADOR GRUPO");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index","Home");    
                }
                
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
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
    }
}
