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
        public async Task<IEnumerable<EstoqueDTO>> GetAllEstoqueDTO(int id)
        {
            var query = from figurinomanequim in _context.Figurinomanequims
                        where figurinomanequim.IdFigurino == id
                        select new EstoqueDTO
                        {
                            IdManequim = figurinomanequim.IdManequim,
                            IdFigurino = figurinomanequim.IdFigurino,
                            Tamanho = figurinomanequim.IdManequimNavigation.Tamanho,
                            Disponivel = figurinomanequim.QuantidadeDisponivel,
                            Entregues = figurinomanequim.QuantidadeEntregue
                        };
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<int> CreateEstoque(Figurinomanequim estoque)
        {
            if(estoque.IdManequim == null || estoque.IdFigurino == null)
            {
                return 400;//falta algum dos id's
            }
            else if(estoque.QuantidadeDisponivel <= 0)
            {
                return 401;//nao existe quantidade para disponibilizar
            }

            try
            {
                var estoqueFound = await _context.Figurinomanequims.FindAsync(estoque.IdFigurino, estoque.IdManequim);

                if(estoqueFound != null)
                {
                    estoqueFound.QuantidadeDisponivel += estoque.QuantidadeDisponivel;

                    _context.Figurinomanequims.Update(estoqueFound);

                    await _context.SaveChangesAsync();

                    return 201; //caso estoque ja existia
                }

                await _context.Figurinomanequims.AddAsync(estoque);
            }
            catch
            {
                return 500;//deu tudo errado
            }


            await _context.SaveChangesAsync();
            return 200;
        }
    }
}
