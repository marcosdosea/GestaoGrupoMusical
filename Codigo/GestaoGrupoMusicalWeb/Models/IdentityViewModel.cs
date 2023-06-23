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

        public class CadastrarViewModel
        {
            public PessoaViewModel Pessoa { get; set; } = new();

            [Required]
            [DataType(DataType.Password)]
            public string Senha { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Senha", ErrorMessage = "As senhas devem ser iguais")]
            public string ConfirmarSenha { get; set; } = string.Empty;

            public Dictionary<string, string> SexoPessoa { get; } = new()
            {
                { "Feminino", "F" },
                { "Masculino", "M" }
            };
        }
    }
}
