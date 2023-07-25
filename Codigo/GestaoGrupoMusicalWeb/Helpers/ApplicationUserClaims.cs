using Core;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace GestaoGrupoMusicalWeb.Helpers
{
    public class ApplicationUserClaims : UserClaimsPrincipalFactory<UsuarioIdentity, IdentityRole>
    {
        private readonly IPessoaService _pessoaService;

        public ApplicationUserClaims(
            UserManager<UsuarioIdentity> userManager, 
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options,
            IPessoaService pessoaService)
            : base(userManager, roleManager, options)
        {
            _pessoaService = pessoaService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UsuarioIdentity user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var pessoa = await _pessoaService.GetByCpf(identity.Name);
            if (pessoa != null)
            {
                identity.AddClaim(new Claim("UserName", pessoa.Nome.Split(" ")[0]));
                identity.AddClaim(new Claim("UserRole", pessoa.Papel.ToLower()));
            }

            return identity;
        }
    }
}
