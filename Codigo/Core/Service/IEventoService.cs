using Core.Datatables;
using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IEventoService
    {
        Task<HttpStatusCode> Create(Evento evento, IEnumerable<int> idRegentes, int idFigurino);
        HttpStatusCode Edit(Evento evento);
        HttpStatusCode Delete(int id);
        Evento? Get(int id);
        ICollection<Eventopessoa> GetEventoPessoasPorIdEvento(int idEvento);
        ICollection<Eventopessoa> GetRegentesEventoPessoasPorIdEvento(int idEvento);
        IEnumerable<Evento> GetAll();
        IEnumerable<EventoDTO> GetAllDTO();
        IEnumerable<EventoIndexDTO> GetAllEventoIndexDTOPerIdGrupoMusical(int idGrupoMusical);
        DatatableResponse<EventoIndexDTO> GetDataPage(DatatableRequest request, int idGrupo);
        HttpStatusCode NotificarEventoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEvento);

        Task<HttpStatusCode> CreateApresentacaoInstrumento(Apresentacaotipoinstrumento apresentacaotipoinstrumento);
    }
}
