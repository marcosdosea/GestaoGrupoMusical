using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class InstrumentoAssociadoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Instrumento")]
        public string NomeInstrumento { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        [Display(Name = "Movimentação")]
        public string Movimentacao { get; set; } = string.Empty;

        public string NomeStatus { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
