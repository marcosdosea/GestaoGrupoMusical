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

        [HttpGet]
        public async Task<IActionResult> Autenticar()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Autenticar(AutenticarViewModel model)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }
    }
}
