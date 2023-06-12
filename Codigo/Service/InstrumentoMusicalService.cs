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

        public int Create(Instrumentomusical instrumentoMusical)
        {
            _context.Add(instrumentoMusical);
            _context.SaveChanges();
            return instrumentoMusical.Id;
        }

        public void Delete(int id)
        {
            var instrumentoMusical = _context.Instrumentomusicals.Find(id);
            _context.Remove(instrumentoMusical);
            _context.SaveChanges();
        }

        public void Edit(Instrumentomusical instrumentoMusical)
        {
            _context.Update(instrumentoMusical);
            _context.SaveChanges();
        }

        public Instrumentomusical Get(int id)
        {
            return _context.Instrumentomusicals.Find(id);
        }

        public IEnumerable<Instrumentomusical> GetAll()
        {
            return _context.Instrumentomusicals.AsNoTracking();
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
            }

            return query;
        }
    }
}
