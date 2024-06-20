using Core.DTO;
using Core.Service;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<HttpStatusCode> Delete(int idGrupoMusical, int idPessoa)
        {
            var informativo = await Get(idGrupoMusical, idPessoa);
            if (informativo == null)
            {
                return HttpStatusCode.NotFound;
            }
            _context.Remove(informativo);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        public async Task<bool> Edit(Informativo informativo)
        {
            try
            {
                _context.Informativos.Update(informativo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Informativo?> Get(int idGrupoMusical, int idPessoa)
        {
            return await _context.Informativos.FindAsync(idGrupoMusical, idPessoa);
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

