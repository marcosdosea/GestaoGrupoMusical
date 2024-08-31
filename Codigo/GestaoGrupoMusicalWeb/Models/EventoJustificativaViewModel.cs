using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EventoJustificativaViewModel
    {
        public int IdEvento { get; set; }

        [Display(Name = "Justificativa de Ausência", Prompt = "Informe a Justificativa...")]
        [MaxLength(200, ErrorMessage = "O Campo {0} deve ter no máximo 100 caracteres")]
        public string? Justificativa { get; set; }
    }
}
