﻿using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class GerenciarInstrumentoEventoDTO
    {
        public int Id { get; set; }
        public int IdInstrumento { get; set; }

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

        [Display(Name = "Instrumentos")]
        public IEnumerable<Tipoinstrumento>? Instrumentos { get; set; }
        
        public int Planejado { get; set; }
        public int Solicitados { get; set; }
        public int Confirmados { get; set; }

    }      
}
