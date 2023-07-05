using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EnsaioIndexDTO
    {
        public int Id { get; set; }
        [DisplayName("Data Hora Inicio")]
        public DateTime? DataHoraInicio { get; set; }
        [DisplayName("Tipo")]
        public string Tipo { get; set; } = null!;
        [DisplayName("Local")]
        public string? Local { get; set; }
        [DisplayName("Presença Obrigatória")]
        public sbyte PresencaObrigatoria { get; set; }
    }
}
