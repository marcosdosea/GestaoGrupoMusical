using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using Core.Datatables;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class FigurinoService : IFigurinoService
    {
        private readonly GrupoMusicalContext _context;

        public FigurinoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> Create(Figurino figurino)
        {
            try
            {
                await _context.Figurinos.AddAsync(figurino);
                await _context.SaveChangesAsync();

                return HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError; //se tudo der errado
            }
        }

        public async Task<HttpStatusCode> Delete(int id)
        {
            try
            {
                var figurino = await Get(id);

                _context.Figurinos.Remove(figurino);

                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }

        }

        public async Task<HttpStatusCode> Edit(Figurino figurino)
        {
            try
            {
                _context.Update(figurino);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<Figurino> Get(int id)
        {
            return await _context.Figurinos.FindAsync(id); ;
        }

        public async Task<IEnumerable<Figurino>> GetAll(int idGrupo)
        {
            var query = await _context.Figurinos.Where(p => p.IdGrupoMusical == idGrupo).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<IEnumerable<FigurinoDropdownDTO>> GetAllFigurinoDropdown(int idGrupo)
        {
            var query = await _context.Figurinos.Where(p => p.IdGrupoMusical == idGrupo)
                .Select(g => new FigurinoDropdownDTO
                {
                    Id = g.Id,
                    Nome = g.Nome
                }).AsNoTracking().ToListAsync();

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

        public async Task<HttpStatusCode> CreateEstoque(Figurinomanequim estoque)
        {
            if (estoque.IdManequim == null || estoque.IdFigurino == null)
            {
                return HttpStatusCode.PreconditionFailed;//falta algum dos id's
            }
            else if (estoque.QuantidadeDisponivel <= 0)
            {
                return HttpStatusCode.BadRequest;//nao existe quantidade para disponibilizar
            }

            try
            {
                var estoqueFound = await _context.Figurinomanequims.FindAsync(estoque.IdFigurino, estoque.IdManequim);

                if (estoqueFound != null)
                {
                    estoqueFound.QuantidadeDisponivel += estoque.QuantidadeDisponivel;

                    _context.Figurinomanequims.Update(estoqueFound);

                    await _context.SaveChangesAsync();

                    return HttpStatusCode.Accepted; //caso estoque ja existia
                }

                await _context.Figurinomanequims.AddAsync(estoque);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;//deu tudo errado
            }


            await _context.SaveChangesAsync();
            return HttpStatusCode.Created;
        }

        public async Task<HttpStatusCode> DeleteEstoque(int idFigurino, int idManequim)
        {
            try
            {
                var estoque = _context.Figurinomanequims.FindAsync(idFigurino, idManequim).Result;

                if (estoque.QuantidadeEntregue > 0)
                {
                    estoque.QuantidadeDisponivel = 0;
                    _context.Figurinomanequims.Update(estoque);
                    await _context.SaveChangesAsync();

                    return HttpStatusCode.BadRequest;
                }
                else
                {
                    _context.Figurinomanequims.Remove(estoque);
                    await _context.SaveChangesAsync();

                    return HttpStatusCode.OK;
                }
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> EditEstoque(Figurinomanequim estoque)
        {
            try
            {
                var estoqueFound = await _context.Figurinomanequims.FindAsync(estoque.IdFigurino, estoque.IdManequim);
                if (estoqueFound != null)
                {
                    estoqueFound.QuantidadeDisponivel = estoque.QuantidadeDisponivel;
                    _context.Figurinomanequims.Update(estoqueFound);
                    _context.SaveChanges();
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.NotFound;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<EstoqueDTO> GetEstoque(int idFigurino, int idManequim)
        {
            var query = from figurinomanequim in _context.Figurinomanequims
                        where figurinomanequim.IdFigurino == idFigurino && figurinomanequim.IdManequim == idManequim
                        select new EstoqueDTO
                        {
                            IdManequim = figurinomanequim.IdManequim,
                            IdFigurino = figurinomanequim.IdFigurino,
                            Tamanho = figurinomanequim.IdManequimNavigation.Tamanho,
                            Disponivel = figurinomanequim.QuantidadeDisponivel,
                            Entregues = figurinomanequim.QuantidadeEntregue
                        };
            return await query.FirstAsync();
        }

        public async Task<DatatableResponse<Figurino>> GetDataPage(DatatableRequest request, int idGrupo)
        {
            var figurinos = await GetAll(idGrupo);
            var totalRecords = figurinos.Count();

            // filtra pelo campos de busca
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                figurinos = figurinos.Where(figurinos => figurinos.Id.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                              || figurinos.Nome.ToLower().Contains(request.Search.GetValueOrDefault("value")));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    figurinos = figurinos.OrderBy(figurinos => figurinos.Nome);
                else
                    figurinos = figurinos.OrderByDescending(figurinos => figurinos.Nome);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("2"))
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    figurinos = figurinos.OrderBy(figurinos => figurinos.Data);
                else
                    figurinos = figurinos.OrderByDescending(figurinos => figurinos.Data);

            int countRecordsFiltered = figurinos.Count();

            figurinos = figurinos.Skip(request.Start).Take(request.Length);
            return new DatatableResponse<Figurino>
            {
                Data = figurinos.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }

        public string GetNomeFigurino(int idEnsaio)
        {
            var figurinoEnsaio = _context.Set<Dictionary<string, object>>("Figurinoensaio")
                .Where(fa => (int)fa["IdEnsaio"] == idEnsaio).AsNoTracking().FirstOrDefault();

            if (figurinoEnsaio != null)
            {
                if (figurinoEnsaio.ContainsKey("IdFigurino") && figurinoEnsaio.ContainsKey("IdEnsaio"))
                {
                    int idFigurino = (int)figurinoEnsaio["IdFigurino"];
                    int idEnsaioResgatado = (int)figurinoEnsaio["IdEnsaio"];

                    string? nomeFigurino = (from figurino in _context.Figurinos
                                           join fe in _context.Set<Dictionary<string, object>>("Figurinoensaio")
                                           on figurino.Id equals (int)fe["IdFigurino"]
                                           where (int)fe["IdEnsaio"] == idEnsaioResgatado && figurino.Id == idFigurino
                                           select figurino.Nome)
                                  .FirstOrDefault();

                    return nomeFigurino ?? "";
                }
            }

            return "";
        }
    }
}
