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
        /// <summary>
        /// Método usado a para adicionar um novo manequim
        /// </summary>
        /// <param name="manequim"></param>
        /// <returns>Id do Manequim</returns>
        public int Create(Manequim manequim)
        {
            _context.Manequims.Add(manequim);
            _context.SaveChanges();

            return manequim.Id;
        }
        /// <summary>
        /// Método que deleta um manequim
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var manequim = _context.Manequims.Find(id);
            _context.Remove(manequim);
            _context.SaveChanges();
        }
        /// <summary>
        /// Metodo usado para editar um manequim
        /// </summary>
        /// <param name="manequim"></param>
        public void Edit(Manequim manequim)
        {
            _context.Update(manequim);
            _context.SaveChanges();
        }
        /// <summary>
        /// Consulta um Manequim no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Manequim correspondente ao id ou um Manequim vazio</returns>
        public Manequim Get(int id)
        {
            return _context.Manequims.Find(id);
        }
        /// <summary>
        /// Consulta todos os Manequins no banco de dados
        /// </summary>
        /// <returns>Uma lista contendo todos os Manequins</returns>
        public IEnumerable<Manequim> GetAll()
        {
            return _context.Manequims.AsNoTracking();
        }
    }
}
