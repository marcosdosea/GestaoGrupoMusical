using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
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
}
