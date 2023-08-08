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

        public async Task<int> Create(Figurino figurino)
        {
            try
            {
                await _context.Figurinos.AddAsync(figurino);
                await _context.SaveChangesAsync();

                return 200;
            }
            catch
            {
                return 500; //se tudo der errado
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                var figurino = await Get(id);

                _context.Figurinos.Remove(figurino);

                await _context.SaveChangesAsync();

                return 200;
            }
            catch
            {
                return 500;
            }
            
        }

        public async Task<int> Edit(Figurino figurino)
        {
            try
            {
                _context.Update(figurino);
                await _context.SaveChangesAsync();

                return 200;
            }
            catch
            {
                return 500;
            }
            

        }

        public async Task<Figurino> Get(int id)
        {
            return await _context.Figurinos.FindAsync(id); ;
        }

        public async Task<IEnumerable<Figurino>> GetAll(string cpf)
        {
            var pessoa = await _context.Pessoas.Where(p => p.Cpf == cpf).SingleOrDefaultAsync();
            int idGrupo = pessoa.IdGrupoMusical;

            var query = await _context.Figurinos.Where(p => p.IdGrupoMusical == idGrupo).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<Figurino> GetByName(string name)
        {
            var figurino = await _context.Figurinos.Where(p => p.Nome == name).AsNoTracking().SingleOrDefaultAsync();

            return figurino;
        }
        public async Task<IEnumerable<Manequim>> GetAllManequim()
        {
            return await _context.Manequims.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<EstoqueDTO>> GetAllEstoqueDTO(int id)
        {
            var query = from figurino in _context.Figurinos
                        where figurino.Id == id
                        join figurinomanequim in _context.Figurinomanequims
                        on figurino.Id equals figurinomanequim.IdFigurino into joinedFigurinoManequim //Criação do grupo com LEFT
                        from figurinoManequim in joinedFigurinoManequim.DefaultIfEmpty()
                        orderby figurinoManequim == null ? null : figurinoManequim.IdManequimNavigation.Tamanho
                        select new EstoqueDTO
                        {
                            Nome = figurino.Nome,
                            Data = figurino.Data,
                            Tamanho = figurinoManequim == null ? null : figurinoManequim.IdManequimNavigation.Tamanho,
                            Disponivel = figurinoManequim == null ? 0 : figurinoManequim.QuantidadeDisponivel,
                            Entregues = figurinoManequim == null ? 0 : figurinoManequim.QuantidadeEntregue
                        };

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
