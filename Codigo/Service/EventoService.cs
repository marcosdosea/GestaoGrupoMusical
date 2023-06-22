using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class EventoService : IEventoService
    {
        private readonly GrupoMusicalContext _context;

        public EventoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método usado a para adicionar uma nova apresentação
        /// </summary>
        /// <param name="evento"></param>
        /// <returns>Id do Grupo Musical</returns>
        public int Create(Evento evento)
        {
            _context.Add(evento);
            _context.SaveChanges();
            return evento.Id;
        }
        /// <summary>
        /// Método que deleta uma apresentação 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var evento = _context.Eventos.Find(id);
            _context.Remove(evento);
            _context.SaveChanges();
        }

        /// <summary>
        /// Metodo usado para editar um Grupo Musical
        /// </summary>
        /// <param name="evento"></param>
        public void Edit(Evento evento)
        {
            _context.Update(evento);
            _context.SaveChanges();

        }

        public Evento Get(int id)
        {
            return _context.Eventos.Find(id);
        }

        public IEnumerable<Evento> GetAll()
        {
            return _context.Eventos.AsNoTracking();
        }

        public IEnumerable<EventoDTO> GetAllDTO()
        {
            var query = _context.Eventos
                .OrderBy(g => g.DataHoraInicio)
                .Select(g =>
                new EventoDTO
                {
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local
                });

            return query.AsNoTracking();
        }
    }
}
