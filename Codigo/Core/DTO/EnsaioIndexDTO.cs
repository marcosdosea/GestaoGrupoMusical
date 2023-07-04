using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EnsaioIndexDTO
    {
        public int Id { get; set; }
        public DateTime? DataHoraInicio { get; set; }
        public string Tipo { get; set; } = null!;
        public string? Local { get; set; }
        public sbyte PresencaObrigatoria { get; set; }
    }
}
