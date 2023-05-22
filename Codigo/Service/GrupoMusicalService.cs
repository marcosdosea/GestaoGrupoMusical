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
    internal class GrupoMusicalService : IGrupoMusical
    {

        private readonly GrupoMusicalContext _context;

        /// <summary>
        /// Metodo usadoa para adicionar o Grupo Musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>Id do Grupo Musical</returns>
        public int Create(Grupomusical grupomusical)
        {
            _context.Add(grupomusical);
            _context.SaveChanges();
            return grupomusical.Id;
        }
        /// <summary>
        /// Metodo para deletar o Grupo Musical
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var grupomusical = _context.Grupomusicals.Find(id);
            _context.Remove(grupomusical);
            _context.SaveChanges();
        }
        /// <summary>
        /// Metodo usado para editar um Grupo Musical
        /// </summary>
        /// <param name="grupomusical"></param>
        public void Edit(Grupomusical grupomusical)
        {
            _context.Update(grupomusical);
            _context.SaveChanges();
           
        }

        public Grupomusical Get(int id)
        {
            return _context.Grupomusicals.Find(id);
        }

        public IEnumerable<Grupomusical> GetAll()
        {
          return _context.Grupomusicals.AsNoTracking();
        }
    }
}
