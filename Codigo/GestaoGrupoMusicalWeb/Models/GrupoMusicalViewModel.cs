using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class GrupoMusicalViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } = null!;
        [Required]
        public string RazaoSocial { get; set; } = null!;
        [Required]
        public string Cnpj { get; set; } = null!;
        public string? Cep { get; set; }
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        [Required]
        public string Estado { get; set; } = null!;
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Youtube { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Banco { get; set; }
        public string? Agencia { get; set; }
        public string? NumeroContaBanco { get; set; }
        public string? ChavePix { get; set; }
        public string? ChavePixtipo { get; set; }
    }
}
