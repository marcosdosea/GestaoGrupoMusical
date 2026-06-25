using GestaoGrupoMusicalAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalAPI.Models
{
    public class IdentityViewModel
    {
        public class AutenticarViewModel
        {
            [Display(Name = "CPF")]
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido")]
            public string Cpf { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [DataType(DataType.Password)]
            public string Senha { get; set; } = string.Empty;
        }

        public class ForgotPasswordViewModel
        {
            [Display(Name = "Email")]
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            public string? Email { get; set; }
        }

        public class ResetPasswordViewModel
        {
            public string? UserId { get; set; }
            public string? Code { get; set; }

            [Display(Name = "Nova Senha")]
            [Required(ErrorMessage = "Campo obrigatório")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Confirmar Senha")]
            [Required(ErrorMessage = "Campo obrigatório")]
            [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
            [Compare("Password", ErrorMessage = "Senhas não são iguais")]
            public string ConfirmePassword { get; set; } = string.Empty;
        }
    }
}
