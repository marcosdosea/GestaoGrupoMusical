using System.ComponentModel.DataAnnotations;
namespace Core.DTO
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public string? Local { get; set; }
        public DateTime DataHoraInicio { get; set; }
    }

    public class EventoIndexDTO
    {
        public int Id { get; set; }
        [Display(Name = "Data hora início")]
        public DateTime DataHoraInicio { get; set; }
        [Display(Name = "Local")]
        public string? Local { get; set; }
        [Display(Name = "Planejados")]
        public int Planejados { get; set; }
        [Display(Name = "Confirmados")]
        public int Confirmados { get; set; }
    }

    public class GerenciarInstrumentoEventoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Data hora início")]
        public DateTime DataHoraInicio { get; set; }

        [Display(Name = "Data hora fim")]
        public DateTime DataHoraFim { get; set; }

        [Display(Name = "Regente")]
        public IEnumerable<string>? Regente { get; set; }

        [Display(Name = "Figurino")]
        public IEnumerable<string>? Figurino { get; set; }

        [Display(Name = "Local")]
        public string? Local { get; set; }

        [Display(Name = "Instrumento")]
        public IEnumerable<Tipoinstrumento>? Instrumento { get; set; }

    }
}
