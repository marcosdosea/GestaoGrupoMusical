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
        public int IdApresentacao { get; set; }
        public int IdInstrumento { get; set; }
        [Display(Name = "Planejados")]
        public int Planejados { get; set; }
        [Display(Name = "Solicitados")]
        public int Solicitados { get; set; }
        [Display(Name = "Confirmados")]
        public int Confirmados { get; set; }

        [Display(Name = "Instrumentos")]
        public string ListaInstrumentos { get; set; } = string.Empty;
    }
}