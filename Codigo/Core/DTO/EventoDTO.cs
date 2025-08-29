using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Core.DTO
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public string? Local { get; set; }
        public DateTime DataHoraInicio { get; set; }
    }

    public class EventoIndexDTO
    {
        public int Id { get; set; }

        [Display(Name = "Data hora início")]
        public DateTime DataHoraInicio { get; set; }
        [Display(Name = "Local")]
        public string? Local { get; set; }
        [Display(Name = "Planejados")]
        public int Planejados { get; set; }
        [Display(Name = "Confirmados")]
        public int Confirmados { get; set; }
    }

    public class GerenciarSolicitacaoEventoDTO
    {
        //esse ID é do evento
        public int Id { get; set; }
        public DateTime DataHoraInicio { get; set; }

        public DateTime DataHoraFim { get; set; }

        public string NomesRegentes { get; set; } = "";

        public int FaltasPessoasEmEnsaioMeses { get; set; }
        public IEnumerable<SolicitacaoEventoPessoasDTO>? EventoSolicitacaoPessoasDTO { get; set; }
    }

    public class SolicitacaoEventoPessoasDTO
    {
        public int IdInstrumento { get; set; }
        public string NomeInstrumento { get; set; } = null!;
        public int IdAssociado { get; set; }
        public int IdPapelGrupo { get; set; }
        public string NomeAssociado { get; set; } = null!;
        public int Faltas { get; set; }
        public int Inadiplencia { get; set; }
        public InscricaoEventoPessoa AprovadoModel { get; set; }
        public InscricaoEventoPessoa Aprovado { get; set; }
    }


    /// <summary>
    /// Tabela EventoPessoa tem o tipo de inscricao.
    /// </summary>
    public enum InscricaoEventoPessoa
    {
        INSCRITO,
        DEFERIDO,
        INDEFERIDO,
        NAO_SOLICITADO
    }

    public class EventosEnsaiosAssociadoDTO
    {
        public IEnumerable<EnsaioAssociadoDTO>? Ensaios { get; set; }
        public IEnumerable<EventoAssociadoDTO>? Eventos { get; set; }
    }


    public class EventoAssociadoDTO
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }

        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        [Display(Name = "Fim")]
        public DateTime Fim { get; set; }
        [Display(Name = "Local")]
        public string? Local { get; set; }


        [Display(Name = "Aprovado")]
        public InscricaoEventoPessoa AprovadoModel { get; set; }
    }

    public class InstrumentoSolicitacaoDTO
    {
        public int IdInstrumento { get; set; }
        public string NomeInstrumento { get; set; } = null!;
        public int QuantidadePlanejada { get; set; }
        public int QuantidadeConfirmada { get; set; }
        public int QuantidadeSolicitada { get; set; }
        public int VagasDisponiveis { get; set; }
    }

    public class EventoPessoaSolicitacaoDTO
    {
        public int IdEvento { get; set; }
        public int IdPessoa { get; set; }
        public int? IdTipoInstrumento { get; set; }
        public string? NomeInstrumento { get; set; }
        public string Status { get; set; } = null!;
        public InscricaoEventoPessoa StatusEnum { get; set; }
    }

    public class SolicitarParticipacaoDTO
    {
        public int IdEvento { get; set; }
        public int IdTipoInstrumento { get; set; }
        public string? Observacoes { get; set; }
    }

    public class EventoDetalhesAssociadoDTO
    {
        public int Id { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string? Local { get; set; }
        public string? Repertorio { get; set; }
        public IEnumerable<InstrumentoSolicitacaoDTO> InstrumentosDisponiveis { get; set; } = new List<InstrumentoSolicitacaoDTO>();
        public EventoPessoaSolicitacaoDTO? MinhaInscricao { get; set; }
        public bool PodeInscrever { get; set; }
        public bool PodeCancelar { get; set; }
    }
}

//ENUM('INSCRITO', 'DEFERIDO', 'INDEFERIDO')
