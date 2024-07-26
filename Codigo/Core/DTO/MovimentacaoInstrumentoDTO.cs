using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class MovimentacaoInstrumentoDTO
    {
        public int Id { get; set; }

        public int IdInstrumento { get; set; }

        [Display (Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        [Display(Name = "Movimentação")]
        public string Movimentacao { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
