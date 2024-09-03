using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class EventoAssociadoDTO
    {
        //id do evento
        public int Id { get; set; }

        public string? Local { get; set; }

        [Display(Name = "Repertório")]
        public string? Repertorio { get; set; }

        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        [Display(Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public string NomesRegentes { get; set; } = string.Empty;
        public string NomeFigurino { get; set; } = string.Empty;

        List<AssociadoDTO>? Associados { get; set; }    
    }
}