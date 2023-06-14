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
    public class ManequimService : IManequim
    {
        private readonly GrupoMusicalContext _context;

        public ManequimService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public int Create(Manequim manequim)
        {
            _context.Manequims.Add(manequim);
            _context.SaveChanges();

            return manequim.Id;
        }

        public void Delete(int id)
        {
            var manequim = _context.Manequims.Find(id);
            _context.Remove(manequim);
            _context.SaveChanges();
        }

        public void Edit(Manequim manequim)
        {
            _context.Update(manequim);
            _context.SaveChanges();
        }

        public Manequim Get(int id)
        {
            return _context.Manequims.Find(id);
        }

        public IEnumerable<Manequim> GetAll()
        {
            return _context.Manequims.AsNoTracking();
        }
    }
}
