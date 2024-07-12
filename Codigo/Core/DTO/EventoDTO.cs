using System.ComponentModel;
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
        [DisplayName("Data hora início")]
        public DateTime DataHoraInicio { get; set; }
        [DisplayName("Local")]
        public string? Local { get; set; }
        [DisplayName("Planejados")]
        public int Planejados { get; set; }
        [DisplayName("Confirmados")]
        public int Confirmados { get; set; }
    }
    public class GerenciarInstrumentoEventoDTO
    {
        public int Id { get; set; }

        [DisplayName("Data hora início")]
        public DateTime DataHoraInicio { get; set; }

        [DisplayName("Data hora fim")]
        public DateTime DataHoraFim { get; set; }

        [DisplayName("Regente")]
        public IEnumerable<string>? Regente { get; set; }

        [DisplayName("Figurino")]
        public IEnumerable<string>? Figurino { get; set; }

        [DisplayName("Local")]
        public string? Local { get; set; }

        [DisplayName("Instrumento")]
        public IEnumerable<Tipoinstrumento>? Instrumento { get; set; }

    }
}
