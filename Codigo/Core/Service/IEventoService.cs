using Core.DTO;

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
    }
}
