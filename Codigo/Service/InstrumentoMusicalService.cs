using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
