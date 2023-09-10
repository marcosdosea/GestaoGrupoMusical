
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Nome", Prompt = "Informe seu nome...")]
        public string Nome { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Sexo { get; set; } = string.Empty;

        [Display(Name = "CEP", Prompt = "00000-000")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Cep { get; set; } = string.Empty;

        [Display(Prompt = "Informe sua rua...")]
        public string? Rua { get; set; }

        [Display(Prompt = "Informe seu bairro...")]
        public string? Bairro { get; set; }

        [Display(Prompt = "Informe sua cidade...")]
        public string? Cidade { get; set; }

        [Display(Name = "UF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Telefone 1", Prompt = "(99)98888-8888")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Telefone1 { get; set; } = string.Empty;

        [Display(Name = "Telefone 2", Prompt = "(99)98888-8888")]
        public string? Telefone2 { get; set; }

        [Display(Name = "E-mail", Prompt = "exemplo@gmail.com")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Email { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdPapelGrupo { get; set; }

        public Dictionary<string, char> sexoPessoa { get; } = new()
        {
            { "Masculino", 'M' },
            { "Feminino", 'F' }
        };
    }
}
