// Core/DTO/EventoDetailsDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class EventoDetailsDTO
    {
        public int Id { get; set; }

        [Display(Name = "Data e Hora de Início")]
        public DateTime DataHoraInicio { get; set; }

        [Display(Name = "Data e Hora de Fim")]
        public DateTime DataHoraFim { get; set; }

        [Display(Name = "Local")]
        public string? Local { get; set; }

        [Display(Name = "Repertório")]
        public string? Repertorio { get; set; }

        [Display(Name = "Regentes")]
        public List<string>? Regentes { get; set; }
    }
}