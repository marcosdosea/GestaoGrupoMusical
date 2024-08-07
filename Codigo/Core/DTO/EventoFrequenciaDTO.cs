﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EventoFrequenciaDTO
    {
        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        public string? Local { get; set; } 

        public string Tipo { get; set; } = string.Empty;

        [Display(Name = "Figurino")]
        public IEnumerable<string>? Figurino { get; set; }
        
        public IEnumerable<string>? Regentes { get; set; }

        public IEnumerable<EventoListaFrequenciaDTO>? Frequencias { get; set; }
    }

    public class EventoListaFrequenciaDTO
    {
        public int IdPessoa { get; set; }

        public int IdEvento { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        [Display(Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public bool Presente { get; set; }

        [Display(Name = "Justificativa Aceita")]
        public bool JustificativaAceita { get; set; }
    }
}

