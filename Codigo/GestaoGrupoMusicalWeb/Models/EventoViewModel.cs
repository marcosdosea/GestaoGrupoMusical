using Core;
using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EventoViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Grupo Musical")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdGrupoMusical { get; set; }
        [Display(Name = "Data hora início")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime DataHoraInicio { get; set; }
        [Display(Name = "Data Hora Fim")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime DataHoraFim { get; set; }
        [Display(Name = "Local")]
        public string? Local { get; set; }
        [Display(Name = "Repetório")]
        public string? Repertorio { get; set; }
        [Display(Name = "Colaborador Reponsável")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdColaboradorResponsavel { get; set; }
        [Display(Name = "Regente")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdRegente { get; set; }

        [Display(Name = "Regentes")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<int>? IdRegentes { get; set; }
        public string? JsonLista { get; set; }
        public IEnumerable<FigurinoDropdownDTO> FigurinoList { get; set; } = null!;

        public SelectList? ListaPessoa { get; set; }
        public SelectList? ListaGrupoMusical { get; set; }       

    }

    public class EventoCreateViewlModel
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }
        public int? IdPessoa { get; set; }

        [Display(Name = "Início")]
        [Required(ErrorMessage = "A data inicial é obrigatório")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Final")]
        [Required(ErrorMessage = "A data final é obrigatório")]
        public DateTime? DataHoraFim { get; set; }

        public SelectList? FigurinoList { get; set; }

        [Display(Name = "Figurino")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdFigurinoSelecionado { get; set; }

        [Display(Name = "Local")]
        public string? Local { get; set; }


        [Display(Name = "Repertório")]
        public string? Repertorio { get; set; }

        [Display(Name = "Regentes")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<int>? IdRegentes { get; set; }
        public string? JsonLista { get; set; }

        public SelectList? ListaPessoa { get; set; }

    }
    public class GerenciarInstrumentoEventoViewModel
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdTipoInstrumento { get; set; }
        public int IdApresentacao { get; set; }
        public int? IdPessoa { get; set; }

        [Display(Name = "Início")]
        [Required(ErrorMessage = "A data inicial é obrigatório")]
        public DateTime? DataHoraInicio { get; set; }

        [Display(Name = "Final")]
        [Required(ErrorMessage = "A data final é obrigatório")]
        public DateTime? DataHoraFim { get; set; }

        public SelectList? FigurinoList { get; set; }

        [Display(Name = "Figurino")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int? IdFigurinoSelecionado { get; set; }

        [Display(Name = "Local")]
        public string? Local { get; set; }
        
        [Display(Name = "Regentes")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public IEnumerable<int>? IdRegentes { get; set; }              

        public string? JsonLista { get; set; }

        public SelectList? ListaPessoa { get; set; }
        
        [Display(Name = "Instrumento")]
        public SelectList? ListaInstrumentos { get; set; }
        public int Quantidade { get; set; }
        public IEnumerable<GerenciarInstrumentoEventoDTO>? GerenciarInstrumentos { get; set; }        

    }
    public class FrequenciaEventoViewModel
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

}
