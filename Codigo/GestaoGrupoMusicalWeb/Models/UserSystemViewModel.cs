using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class UserSystemViewModel
    {
        [Display(Name = "Senha Atual")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "Nova Senha")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Confirmar Senha")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
        [Compare("Password", ErrorMessage = "Senhas não são iguais")]
        [DataType(DataType.Password)]
        public string? ConfirmePassword { get; set; }
    }
}
