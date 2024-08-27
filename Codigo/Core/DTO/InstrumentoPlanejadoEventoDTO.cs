using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class InstrumentoPlanejadoEventoDTO
    {
        public int Id { get; set; }
        public int IdInstrumento { get; set; }

        [Display(Name = "Instrumentos")]
        public IEnumerable<Tipoinstrumento>? Instrumentos { get; set; }
        public int Quantidade { get; set; }
    }
}