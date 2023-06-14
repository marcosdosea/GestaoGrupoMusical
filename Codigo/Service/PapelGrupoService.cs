using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PapelGrupoService : IPapelGrupo
    {
        private readonly GrupoMusicalContext _context;

        public PapelGrupoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public int Create(Papelgrupo papel)
        {
            _context.Papelgrupos.Add(papel);
            _context.SaveChanges();

            return papel.IdPapelGrupo;
        }

        public void Delete(int id)
        {
            var papel = _context.Papelgrupos.Find(id);
            _context.Remove(papel);
            _context.SaveChanges();
        }

        public void Edit(Papelgrupo papel)
        {
            _context.Update(papel);
            _context.SaveChanges();
        }

        public Papelgrupo Get(int id)
        {
            return _context.Papelgrupos.Find(id);
        }

        public IEnumerable<Papelgrupo> GetAll()
        {
            return _context.Papelgrupos.AsNoTracking();
        }
    }
}
