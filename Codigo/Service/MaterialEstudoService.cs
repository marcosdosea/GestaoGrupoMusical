using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MaterialEstudoService : IMaterialEstudoService
    {
        private readonly GrupoMusicalContext _context;
        public MaterialEstudoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(Materialestudo materialEstudo)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                await _context.AddAsync(materialEstudo);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var materialEstudo = await Get(id);
            if (materialEstudo == null)
            {
                return false;
            }
            _context.Remove(materialEstudo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(Materialestudo materialEstudo)
        {
            try
            {
                _context.Update(materialEstudo);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                return false;
            }
        }

        public async Task<Materialestudo?> Get(int id)
        {
            return await _context.Materialestudos.FindAsync(id);
        }

        public async Task<IEnumerable<Materialestudo?>> GetAll()
        {
            return await _context.Materialestudos.AsNoTracking().ToListAsync();
        }
    }
}
