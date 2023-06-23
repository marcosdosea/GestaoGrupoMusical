using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class IdentityViewModel
    {
        public class AutenticarViewModel
        {
            [Required]
            public string Cpf { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Senha { get; set; } = string.Empty;
        }
    }
}
