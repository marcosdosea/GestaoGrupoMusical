

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class InstrumentoMusicalViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Patrimônio")]
        [Required(ErrorMessage = "O campo Patrimonio é obrigatório")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "O nome do associado deve ter entre 5 e 20 caracteres")]
        public string Patrimonio { get; set; }

        [Display(Name ="Data Aquisição")]
        [Required(ErrorMessage ="A data é obrigatória")]
        public DateTime DataAquisicao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "DISPONIVEL";

        [Display(Name = "Instrumento")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdTipoInstrumento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdGrupoMusical { get; set; }

        public SelectList? ListaInstrumentos { get; set; }

        [Display(Name = "Danificado")]
        public bool? IsDanificado { get; set; } = null; 
    }
}
