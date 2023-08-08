using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EnsaioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Tipo Tipo { get; set; }

        [Display(Name = "Data e Hora de Início")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Data e Hora de Término")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraFim { get; set; }

        [Display(Name = "Presença Obrigatória")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool PresencaObrigatoria { get; set; } = true;
        [Display(Prompt = "Informe o local do ensaio.")]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo 100 caracteres")]
        public string? Local { get; set; }

        [Display(Name = "Repertório", Prompt = "Informe o repertório.")]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo 1000 caracteres")]
        public string? Repertorio { get; set; }

        [Display(Name = "Regente")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdRegente { get; set; }

        public Dictionary<string, bool> Obrigatorio { get; } = new()
        {
            { "Sim", true },
            { "Não", false }
        };

        public SelectList? ListaPessoa { get; set; }
    }

    public enum Tipo 
    { 
        Fixo,
        Extra
    }
}
