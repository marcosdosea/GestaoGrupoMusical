﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.DTO
{
    public class EnsaioDTO
    {
        public int Id { get; set; }
        public string? Local { get; set; }
        public DateTime? DataHoraInicio { get; set; }

    }

    public class EnsaioDetailsDTO
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }

        [Display(Name = "Data e Hora de Início")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Data e Hora de Término")]
        public DateTime? DataHoraFim { get; set; }

        [Display(Name = "Presença Obrigatória")]
        public string? PresencaObrigatoria { get; set; }
        [Display(Prompt = "Informe o local do ensaio.")]
        public string? Local { get; set; }

        [Display(Name = "Repertório", Prompt = "Informe o repertório.")]
        public string? Repertorio { get; set; }

        public IEnumerable<string>? Regentes { get; set; }

        public int? IdGrupoMusical { get;  set; }
    }

    public class AutoCompleteRegenteDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }

    public class EnsaioAssociadoDTO
    {
        public int Id { get; set; }

        [Display(Name = "Local")]
        public string? Local { get; set; }

        [Display(Name = "Repertório")]
        public string? Repertorio { get; set; }

        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        [Display(Name = "Fim")]
        public DateTime Fim { get; set; }

        [Display(Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public bool Presente { get; set; }

        [Display(Name = "Justificativa Aceita")]
        public bool JustificativaAceita { get; set; }
    }
}
