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
        
        [Display(Name = "Figurino")]
        public string? FigurinoApresentacao { get; set; }

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
        public string? FigurinoApresentacao { get; set; }

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
        
        public List<GerenciarInstrumentoEventoDTO> InstrumentosPlanejadoEventoDTO { get; set; }

        public int Planejados { get; set; }
        public int Solicitados { get; set; }
        public int Confirmados { get; set; }
    }

    public class PlanejarInstrumentoEventoViewModel
    {
        public int Id { get; set; }

        public int IdInstrumento { get; set; }

        [Display(Name = "Instrumento")]
        public SelectList? ListaInstrumentos { get; set; }
        public int Quantidade { get; set; }       

        public int Planejados { get; set; }
        public int Solicitados { get; set; }
        
        public int Confirmados { get; set; }

        public IEnumerable<InstrumentoPlanejadoEventoDTO>? InstrumentoEventoPlanejado { get; set; }
    }

        public class GerenciarSolicitacaoEventoViewModel
    {
        //esse ID é do evento
        public int Id { get; set; }


        [Display(Name = "Início")]
        public DateTime DataHoraInicio { get; set; }


        [Display(Name = "Fim")]
        public DateTime DataHoraFim { get; set; }

        [Display(Name = "Regentes")]
        public string NomesRegentes { get;set; } = null!;

        public int FaltasPessoasEmEnsaioMeses { get; set; }
        public IEnumerable<SolicitacaoEventoPessoasDTO>? EventoSolicitacaoPessoasDTO { get; set; }
    }

}
