using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Service
{
    public class InformativoService : IInformativoService
    {
        private readonly GrupoMusicalContext _context;
        public InformativoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> Create(Informativo informativo)
        {
            try
            {
                await _context.Informativos.AddAsync(informativo);
                await _context.SaveChangesAsync();
                return HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public HttpStatusCode Delete(uint id)
        {
            var informativo = Get(id);
            if (informativo == null)
            {
                return HttpStatusCode.NotFound;
            }
            _context.Remove(informativo);
            _context.SaveChanges();
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Edit(Informativo informativo)
        {
            try
            {
                informativo.Data = DateTime.Now;
                _context.Informativos.Update(informativo);
                _context.SaveChanges();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.MethodNotAllowed;
            }
        }

        public Informativo? Get(uint id)
        {
            return _context.Informativos.Find(id);
        }

        public async Task<IEnumerable<Informativo>> GetAll()
        {
            return await _context.Informativos.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<Informativo>> GetAllInformativoServiceIdGrupo(int idGrupoMusical, int idPessoa)
        {
            var query = await (from informativoService in _context.Informativos
                               where informativoService.IdGrupoMusical == idGrupoMusical && informativoService.IdPessoa == idPessoa
                               select informativoService).ToListAsync();

            return query;
        }


    }
}

