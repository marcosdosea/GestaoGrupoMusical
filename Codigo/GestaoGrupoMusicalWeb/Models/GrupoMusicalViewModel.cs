using System.ComponentModel.DataAnnotations;
using Util;

namespace GestaoGrupoMusicalWeb.Models
{
    public class GrupoMusicalViewModel
    {
        [Key]
        [Display(Name = "Codigo")]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Nome", Prompt = "Nome")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O Campo Nome deve ter entre 5 a 100 caracteres")]
        public string Nome { get; set; } = null!;

        [Required]
        [Display(Name = " Razão Social", Prompt = "Razão social")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Razão Social deve ter entre 5 a 100 caracteres")]
        public string RazaoSocial { get; set; } = null!;

        [Required]
        [CNPJ]
        [Display(Name = "CNPJ", Prompt = "00.000.000/0000-00")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$", ErrorMessage = "CNPJ inválido")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "O campo CNPJ deve ter 14 caracteres")]
        public string Cnpj { get; set; } = null!;

        [Display(Name = "CEP", Prompt = "00000-000")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "O campo CEP deve ter 8 caracteres")]
        public string? Cep { get; set; }

        [Display(Name = "Rua", Prompt = "Rua")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo rua deve ter entre 5 a 100 caracteres")]
        public string? Rua { get; set; }

        [Display(Name = "Bairro", Prompt = "Bairro")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Bairro deve ter entre 5 a 100 caracteres")]
        public string? Bairro { get; set; }

        [Display(Name = "Cidade", Prompt = "Cidade")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Cidade de ter entre 5 a 100 caracteres")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório")]
        [Display(Name = "Estado", Prompt = "Estado")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O Campo Estado deve ter 2 dígitos")]
        public string Estado { get; set; } = null!;

        [Display(Name = "Pais", Prompt = "Pais")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = " O campo Pais deve ter de 4 a 50 caracteres")]
        public string? Pais { get; set; }

        [Display(Name = "E-mail", Prompt = "exemplo@gmail.com")]
        [EmailAddress(ErrorMessage = "Endereço de Email Inválido")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "O campo E-mail deve ter de 10 a 100 caracteres")]
        public string? Email { get; set; }

        [Display(Name = "YouTube", Prompt = "YouTube")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo YouTube deve ter de 5 a 100 caracteres")]
        public string? Youtube { get; set; }

        [Display(Name = "Instagram", Prompt = "Instagram")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Instagram deve ter de 5 a 100 caracteres")]
        public string? Instagram { get; set; }

        [Display(Name = "Facebook", Prompt = "Facebook")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Facebook deve ter de 5 a 100 caracteres")]
        public string? Facebook { get; set; }

        [Display(Name = "Telefone 1", Prompt = "(99)99999-9999")]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [StringLength(20)]
        public string? Telefone1 { get; set; }

        [Display(Name = " Telefone 2", Prompt = "(99)99999-9999")]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [StringLength(20)]
        public string? Telefone2 { get; set; }

        [Display(Name = "Banco", Prompt = "Banco")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O campo Banco deve ter de 5 a 100 caracteres")]
        public string? Banco { get; set; }

        [Display(Name = "Agência", Prompt = "Agência")]
        [StringLength(15)]
        public string? Agencia { get; set; }

        [Display(Name = "Número da Conta", Prompt = "Número da Conta bancária")]
        [StringLength(15)]
        public string? NumeroContaBanco { get; set; }

        [Display(Name = "Chave Pix", Prompt = "Chave pix")]
        [StringLength(100)]
        public string? ChavePix { get; set; }

        [Display(Name = " Tipo da cave Pix")]
        [StringLength(15)]
        public string? ChavePixtipo { get; set; }

        public Dictionary<string, string> TipoChave { get; } = new()
        {
            { "Chave aleatória", "chave aleatoria"},
            { "CPF", "cpf"},
            { "Celular", "celular" },
            { "E-mail", "email" }
        };

    }
}
