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
        INDEFERIDO
    }
}


public class FigurinoApresentacaoDTO
{
    public int IdFigurino { get; set; }
    public int IdApresentacao { get; set; }
}
//ENUM('INSCRITO', 'DEFERIDO', 'INDEFERIDO')
