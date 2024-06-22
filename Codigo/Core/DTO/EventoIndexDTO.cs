using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EventoIndexDTO
    {
        public int Id { get; set; }
        [DisplayName("Data hora início")]
        public DateTime DataHoraInicio { get; set; }
        [DisplayName("Local")]
        public string? Local { get; set; }
        [DisplayName("Repertório")]
        public string? Repertorio { get; set; }

    }
}
