using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EnsaioJustificativaViewModel
    {
        public int IdEnsaio { get; set; }

        [Display(Name = "Justificativa de Ausência", Prompt = "Informe a Justificativa...")]
        public string Justificativa { get; set; } = string.Empty;
    }
}
