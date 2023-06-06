

using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class InstrumentoMusicalViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Patrimonio { get; set; }
        [Required]
        public DateTime DataAquisicao { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int IdTipoInstrumento { get; set; }
        [Required]
        public int IdGrupoMusical { get; set; }
    }
}
