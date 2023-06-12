﻿using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EventoViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int IdGrupoMusical { get; set; }
        [Required]
        public DateTime DataHoraInicio { get; set; }
        [Required]
        public DateTime DataHoraFim { get; set; }
        public string? Local { get; set; }
        public string? Repertorio { get; set; }
        public int IdColaboradorResponsavel { get; set; }
        public int IdRegente { get; set; }
    }
}
