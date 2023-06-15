using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

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
            await _context.AddAsync(instrumentoMusical);
            await _context.SaveChangesAsync();
            return instrumentoMusical.Id;
        }

        public async Task Delete(int id)
        {
            _context.Remove(await Get(id));
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Instrumentomusical instrumentoMusical)
        {
            _context.Update(instrumentoMusical);
            await _context.SaveChangesAsync();
        }

        public async Task<Instrumentomusical> Get(int id)
        {
            return await _context.Instrumentomusicals.FindAsync(id) ?? new Instrumentomusical();
        }

        public async Task<IEnumerable<Instrumentomusical>> GetAll()
        {
            return await _context.Instrumentomusicals.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO()
        {
            var query = await   (from instrumento in _context.Instrumentomusicals
                                join movimentacao in _context.Movimentacaoinstrumentos
                                on instrumento.Id equals movimentacao.IdInstrumentoMusical into intMovi
                                from instrumentoMovi in intMovi.DefaultIfEmpty()
                                select new InstrumentoMusicalDTO
                                {
                                    Id = instrumento.Id,
                                    Patrimonio = instrumento.Patrimonio,
                                    NomeInstrumento = instrumento.IdTipoInstrumentoNavigation.Nome,
                                    Status = instrumento.Status,
                                    NomeAssociado = instrumentoMovi.IdAssociadoNavigation.Nome
                                }).AsNoTracking().ToListAsync();

            foreach (var instrumento in query)
            {
                instrumento.Status = instrumento.EnumStatus.Single(s => s.Key == instrumento.Status).Value;
                if (instrumento.Status == "DISPONIVEL")
                {
                    instrumento.NomeAssociado = "";
                }
            }

            return query;
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
