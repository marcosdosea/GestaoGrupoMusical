using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EnsaioViewModel
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Tipo Tipo { get; set; }

        [Display(Name = "Início")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Final")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraFim { get; set; }

        [Display(Name = "Presença Obrigatória")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public bool PresencaObrigatoria { get; set; } = true;

        [Display(Name = "Figurino")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdFigurinoSelecionado { get; set; }

        [Display(Prompt = "Informe o local do ensaio.")]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo 100 caracteres")]
        public string? Local { get; set; }

        [Display(Name = "Repertório", Prompt = "Informe o repertório.")]
        [MaxLength(1000, ErrorMessage = "O campo {0} deve ter no máximo 1000 caracteres")]
        public string? Repertorio { get; set; }

        [Display(Name = "Regentes")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<int>? IdRegentes { get; set; }

        public string? JsonLista { get; set; }

        public Dictionary<string, bool> Obrigatorio { get; } = new()
        {
            { "Sim", true },
            { "Não", false }
        };

        public SelectList? ListaPessoa { get; set; }
        public SelectList? ListaFigurino { get; set; }
    }

    public class FrequenciaEnsaioViewModel
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }

        [Display(Name = "Início")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Final")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime? DataHoraFim { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Tipo Tipo { get; set; }

        [Display(Name = "Regentes")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<int>? IdRegentes { get; set; }

        public SelectList? ListaPessoa { get; set; }

        [Display(Name = "Figurino")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdFigurinoSelecionado { get; set; }

        public SelectList? ListaFigurino { get; set; }

        [Display(Name = "Local")]
        [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo 100 caracteres")]
        public string? Local { get; set; }

        public string? JsonLista { get; set; }
    }

    public enum Tipo 
    { 
        Fixo,
        Extra
    }
}
