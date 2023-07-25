using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace GestaoGrupoMusicalWeb.Helpers
{
    public class ApplicationUserClaims : UserClaimsPrincipalFactory<UsuarioIdentity, IdentityRole>
    {
        public ApplicationUserClaims(
            UserManager<UsuarioIdentity> userManager, 
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UsuarioIdentity user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("UserName","Teste"));

            return identity;
        }
    }
}
