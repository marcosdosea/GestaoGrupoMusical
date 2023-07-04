using Core.DTO;
using Core.Service;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class InformativoService : IInformativoService
    {
            private readonly GrupoMusicalContext _context;
            public InformativoService(GrupoMusicalContext context)
            {
                _context = context;
            }

        public async Task<bool> Create(Informativo informativo)
        {
            try
            {
                await _context.Informativos.AddAsync(informativo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(int idGrupoMusical, int idPessoa)
        {
            try
            {
                _context.Informativos.Remove(await Get(idGrupoMusical, idPessoa));
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task<Informativo> Get(int idGrupoMusical, int idPessoa)
        {
            return await _context.Informativos.FindAsync(idGrupoMusical, idPessoa) ?? new Informativo();
        }

        public async Task<IEnumerable<Informativo>> GetAll()
        {
            return await _context.Informativos.AsNoTracking().ToListAsync();
        }
    }
    }

