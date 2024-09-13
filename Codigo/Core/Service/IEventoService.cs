using Core.Datatables;
using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IEventoService
    {
        Task<Eventopessoa?> GetEventoPessoaAsync(int idEvento, int idPessoa);
        Task<HttpStatusCode> RegistrarFrequenciaAsync(List<EventoListaFrequenciaDTO> listaFrequencia);
        Task<HttpStatusCode> RegistrarJustificativaAsync(int idEvento, int idPessoa, string? justificativa);
        Task<IEnumerable<int>> GetIdRegentesEventoAsync(int idEvento);
        Task<HttpStatusCode> Create(Evento evento, IEnumerable<int> idRegentes, int idFigurino);
        HttpStatusCode Delete(int id);
        HttpStatusCode Edit(Evento evento);
        Evento Get(int id);
        ICollection<Eventopessoa> GetEventoPessoasPorIdEvento(int idEvento);
        IEnumerable<Evento> GetAll();
        IEnumerable<EventoDTO> GetAllDTO();
        IEnumerable<EventoIndexDTO> GetAllIndexDTO();
        IEnumerable<EventoIndexDTO> GetAllEventoIndexDTOPerIdGrupoMusical(int idGrupoMusical);
        DatatableResponse<EventoIndexDTO> GetDataPage(DatatableRequest request, int idGrupo);
        HttpStatusCode NotificarEventoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEvento);
        Task<string> GetNomeInstrumento(int id);
        Task<IEnumerable<FigurinoDropdownDTO>> GetAllFigurinoDropdown(int idGrupo);
        Task<IEnumerable<Eventopessoa>> GetPessoas(int idGrupo);
        Task<HttpStatusCode> CreateApresentacaoInstrumento(Apresentacaotipoinstrumento apresentacaotipoinstrumento);
        GerenciarSolicitacaoEventoDTO? GetSolicitacoesEventoDTO(int idEvento, int pegarFaltasEmMesesAtras);
        IEnumerable<SolicitacaoEventoPessoasDTO> GetSolicitacaoEventoPessoas(int idEvento, int pegarFaltasEmMesesAtras);
        public EventoStatus EditSolicitacoesEvento(GerenciarSolicitacaoEventoDTO g);        

        public static InscricaoEventoPessoa ConvertAprovadoParaEnum(string aprovado)
        {
            if (aprovado.ToLower().Contains("indeferido"))
                return InscricaoEventoPessoa.INDEFERIDO;
            if (aprovado.ToLower().Contains("deferido"))
                return InscricaoEventoPessoa.DEFERIDO;
            return InscricaoEventoPessoa.INSCRITO;
        }
        public enum EventoStatus
        {
            Success,
            UltrapassouLimiteQuantidadePlanejada,
            ErroGenerico,
            SemAlteracao,
            QuantidadeConfirmadaNegativa,
            QuantidadeSolicitadaNegativa
        }
        IEnumerable<InstrumentoPlanejadoEventoDTO> GetInstrumentosPlanejadosEvento(int idEvento);
    }
}