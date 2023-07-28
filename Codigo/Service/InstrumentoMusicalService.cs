using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace Service
{
    public class InstrumentoMusicalService : IInstrumentoMusicalService
    {
        private readonly GrupoMusicalContext _context;

        public InstrumentoMusicalService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Instrumentomusical instrumentoMusical)
        {
            if (instrumentoMusical.DataAquisicao > DateTime.Now)
            {
                return 100;
            }
            try
            {
                await _context.AddAsync(instrumentoMusical);
                await _context.SaveChangesAsync();
                return 200;
            }
            catch (Exception)
            {
                return 500;
            }

        }

        public async Task<int> Delete(int id)
        {
            var instrumento = await Get(id);
            if(instrumento == null)
            {
                return 404;
            }
            try
            {
                var hasMovimentacao = await _context.Movimentacaoinstrumentos.Where(m => m.IdInstrumentoMusical == id).AsNoTracking().AnyAsync();
                if (hasMovimentacao)
                {
                    return 401;
                }
                _context.Remove(instrumento);
                await _context.SaveChangesAsync();
                return 200;
            }
            catch
            {
                return 500;
            }
        }

        public async Task<int> Edit(Instrumentomusical instrumentoMusical)
        {
            try
            {
                _context.Update(instrumentoMusical);
                await _context.SaveChangesAsync();
                return 200;
            }
            catch
            {
                return 500;
            }

        }

        public async Task<Instrumentomusical?> Get(int id)
        {
            return await _context.Instrumentomusicals.FindAsync(id);
        }

        public async Task<IEnumerable<Instrumentomusical>> GetAll()
        {
            return await _context.Instrumentomusicals.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO(int idGrupo)
        {
            var query = await (from instrumento in _context.Instrumentomusicals join
                               movimentacao in _context.Movimentacaoinstrumentos
                               on instrumento.Id equals movimentacao.IdInstrumentoMusical into intMovi
                               from instrumentoMovi in intMovi.DefaultIfEmpty()
                               where instrumento.IdGrupoMusical == idGrupo
                               orderby instrumentoMovi.Data descending
                               select new InstrumentoMusicalDTO
                               {
                                   Id = instrumento.Id,
                                   Patrimonio = instrumento.Patrimonio,
                                   NomeInstrumento = instrumento.IdTipoInstrumentoNavigation.Nome,
                                   Status = instrumento.Status,
                                   NomeAssociado = instrumentoMovi.IdAssociadoNavigation.Nome
                               }).AsNoTracking().ToListAsync();


            var list = query.DistinctBy(m => m.Patrimonio).OrderBy(m => m.NomeInstrumento);

            foreach (var instrumento in list)
            {
                if (instrumento.Status == "DISPONIVEL")
                {
                    instrumento.NomeAssociado = "";
                }
                instrumento.Status = instrumento.EnumStatus.Single(s => s.Key == instrumento.Status).Value;
            }

            return list;
        }

        public async Task<IEnumerable<Tipoinstrumento>> GetAllTipoInstrumento()
        {
            return await _context.Tipoinstrumentos.AsNoTracking().ToListAsync();
        }

        public async Task<string> GetNomeInstrumento(int id)
        {
            var query = await (from instrumento in _context.Instrumentomusicals
                               where instrumento.Id == id
                               select new { instrumento.IdTipoInstrumentoNavigation.Nome }).AsNoTracking().SingleOrDefaultAsync();

            return query?.Nome ?? "";
        }
    }
}
