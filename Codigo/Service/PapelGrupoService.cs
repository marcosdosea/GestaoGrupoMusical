using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PapelGrupoService : IPapelGrupoService
    {

        private readonly GrupoMusicalContext _context;

        public PapelGrupoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pegar todos os papeis 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Papelgrupo> GetAll()
        {
            return _context.Papelgrupos.AsNoTracking();
        }
    }
}
