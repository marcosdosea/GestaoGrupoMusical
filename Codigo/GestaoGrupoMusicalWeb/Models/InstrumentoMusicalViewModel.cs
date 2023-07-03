

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class InstrumentoMusicalViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Patrimônio")]
        [Required]
        public string Patrimonio { get; set; }

        [Display(Name ="Data Aquisição")]
        [Required]
        public DateTime DataAquisicao { get; set; }

        [Required]
        public string Status { get; set; } = "DISPONIVEL";

        [Display(Name = "Instrumento")]
        [Required]
        public int IdTipoInstrumento { get; set; }

        [Required]
        public int IdGrupoMusical { get; set; }

        public SelectList? ListaInstrumentos { get; set; }
        public bool IsDanificado { get; set; } = true;
    }
}
