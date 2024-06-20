using Core.Datatables;
using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IEventoService
    {
        int Create(Evento evento);
        void Edit(Evento evento);
        void Delete(int id);
        Evento Get(int id);
        IEnumerable<Evento> GetAll();
        IEnumerable<EventoDTO> GetAllDTO();
        IEnumerable<EventoIndexDTO> GetAllIndexDTO();
        IEnumerable<EventoIndexDTO> GetAllEventoIndexDTOPerIdGrupoMusical(int idGrupoMusical);
        DatatableResponse<EventoIndexDTO> GetDataPage(DatatableRequest request, int idGrupo);
        HttpStatusCode NotificarEventoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEvento);
    }
}
