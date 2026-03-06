using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly SignInManager<UsuarioIdentity> signInManager;
        private readonly UserManager<UsuarioIdentity> userManager;
        private readonly IConfiguration configuration;
        private readonly GrupoMusicalContext context;

        public IdentityController(
            SignInManager<UsuarioIdentity> signInManager,
            UserManager<UsuarioIdentity> userManager,
            GrupoMusicalContext context, 
            IConfiguration configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context; 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // O web usa CPF como nome de usuário
            var result = await signInManager.PasswordSignInAsync(model.Cpf ?? "", model.Senha ?? "", false, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(model.Cpf ?? "");
                if (user == null) return Unauthorized();

                var token = await GerarTokenJwt(user);
                return Ok(new { token });
            }

            return Unauthorized("CPF ou senha inválidos.");
        }

        private async Task<string> GerarTokenJwt(UsuarioIdentity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = configuration["Jwt:ChaveSecreta"] ?? "CHAVE_PADRAO_COM_MAIS_DE_32_CARACTERES";
            var key = Encoding.ASCII.GetBytes(secret);

            var roles = await userManager.GetRolesAsync(user);

            var pessoaLogada = context.Pessoas.FirstOrDefault(p => p.Cpf == user.UserName);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("Id", user.Id),
            };

            if (pessoaLogada != null)
            {
                claims.Add(new Claim("IdGrupoMusical", pessoaLogada.IdGrupoMusical.ToString()));
                claims.Add(new Claim("IdPessoa", pessoaLogada.Id.ToString()));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = configuration["Jwt:Emissor"],
                Audience = configuration["Jwt:Audiencia"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string? Cpf { get; set; } = string.Empty;
        public string? Senha { get; set; } = string.Empty;
    }
}