﻿using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class EventoAssociadoDTO
    {
        public int IdEvento { get; set; }

        public string? Local { get; set; }

        [Display(Name = "Repertório")]
        public string? Repertorio { get; set; }

        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        [Display(Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public bool Presente { get; set; }

        [Display(Name = "Justificativa Aceita")]
        public bool JustificativaAceita { get; set; }
    }
}

