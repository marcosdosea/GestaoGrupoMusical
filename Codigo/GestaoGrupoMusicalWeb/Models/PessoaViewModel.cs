using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Util;

namespace GestaoGrupoMusicalWeb.Models
{
    public class PessoaViewModel
    {
        [Required(ErrorMessage = "O código do associado é obrigatótio.")]
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [CPF(ErrorMessage = "Esse CPF já esta Cadastrado no Sistema")]
        [Required(ErrorMessage = "O campo de CPF é obrigatório.")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido")]
        [StringLength(15)]
        [Display(Name = "CPF", Prompt = "000.000.000-00")]
        public string? Cpf { get; set; }

        [Display(Name = "Nome", Prompt = "Meu Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatótio.")]
        [StringLength(70, MinimumLength = 5, ErrorMessage = "O nome do associado deve ter entre 5 e 70 caracteres")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
        [StringLength(1)]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
        [StringLength(10)]
        [Display(Name = "CEP", Prompt = "00000-000")]
        public string? Cep { get; set; }

        [StringLength(70)]
        [Display(Name = "Rua", Prompt = "Rua")]
        public string? Rua { get; set; }

        [StringLength(70)]
        [Display(Name = "Bairro", Prompt = "Rua")]
        public string? Bairro { get; set; }

        [StringLength(70)]
        [Display(Name = "Cidade", Prompt = "Cidade")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [StringLength(2)]
        [Display(Name = "Estado", Prompt = "Estado")]
        public string? Estado { get; set; }

        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date, ErrorMessage = "É necessário escolher uma data válida.")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "O campo telefone é obrigatório.")]
        [StringLength(20)]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [Display(Name = "Telefone 1", Prompt = "(99)99999-9999")]
        public string? Telefone1 { get; set; }

        [Display(Name = "Telefone 2", Prompt = "(99)99999-9999")]
        [StringLength(20)]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        public string? Telefone2 { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email está no formato inválido.")]
        [Display(Name = "E-mail", Prompt = "exemplo@gmail.com")]
        public string? Email { get; set; }

        [Display(Name = "Data de entrada")]
        public DateTime? DataEntrada { get; set; }

        [Display(Name = "Data de saída")]
        public DateTime? DataSaida { get; set; }

        [Display(Name = "Motivo da saída")]
        public string? MotivoSaida { get; set; }

        [Required(ErrorMessage = "O campo Ativo é obrigatório.")]
        [Display(Name = "Ativo")]
        public sbyte Ativo { get; set; }
        [Required(ErrorMessage = "O campo isento de pagamento é obrigatório.")]
        [Display(Name = "Isento de pagamento")]
        public sbyte IsentoPagamento { get; set; }

        [Required(ErrorMessage = "O campo Tamanho da roupa é obrigatório.")]
        [Display(Name = "Tamanho da roupa")]
        public int IdManequim { get; set; }

        public SelectList? ListaGrupoMusical { get; set; }
        public SelectList? ListaPapelGrupo { get; set; }
        public SelectList? ListaManequim { get; set; }
        public Dictionary<string, char> sexoPessoa { get; } = new()
        {
            { "Masculino", 'M' },
            { "Feminino", 'F' }
        };



    }
   
}
