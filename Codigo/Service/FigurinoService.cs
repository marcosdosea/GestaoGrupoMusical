using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FigurinoService : IFigurinoService
    {
        private readonly GrupoMusicalContext _context;

        public FigurinoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<int> Create(FigurinoDTO figurinoDto)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Figurino figurino = new();
                Figurinomanequim figurinoManequim = new();

                figurino.Nome = figurinoDto.Nome;
                figurino.Data = figurinoDto.Data;
                figurino.IdGrupoMusical = figurinoDto.IdGrupoMusical;

                await _context.Figurinos.AddAsync(figurino);

                int entitiesAffected = await _context.SaveChangesAsync();

                if(entitiesAffected == 0 )
                {
                    await transaction.RollbackAsync();
                    return 501; //não foi possivel gravar a entidade figurino
                }

                figurino = await GetByName(figurino.Nome);

                figurinoManequim.IdFigurino = figurino.Id;
                figurinoManequim.IdManequim = figurinoDto.IdManequim;
                figurinoManequim.QuantidadeDisponivel = figurinoDto.QuantidadeDisponivel;
                figurinoManequim.QuantidadeEntregue = figurinoDto.QuantidadeEntregue;

                await _context.Figurinomanequims.AddAsync(figurinoManequim);

                entitiesAffected = await _context.SaveChangesAsync();

                if (entitiesAffected == 0)
                {
                    await transaction.RollbackAsync();
                    return 502; //não foi possivel gravar a entidade figurinomanequim
                }


                await transaction.CommitAsync();
                return 200;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 500; //se der tudo errado
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> Edit(FigurinoDTO figurinoDto)
        {
            throw new NotImplementedException();
        }

        public FigurinoDTO Get(int id)
        {

            throw new NotImplementedException();
        }

        public Task<IEnumerable<FigurinoDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Figurino> GetByName(string name)
        {
            var figurino = await _context.Figurinos.Where(p => p.Nome == name).AsNoTracking().SingleOrDefaultAsync();

            return figurino;
        }
    }
}
