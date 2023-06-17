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
    public class PapelGrupoService : IPapelGrupoService
    {
        private readonly GrupoMusicalContext _context;

        public PapelGrupoService(GrupoMusicalContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Método usado a para adicionar um novo Papel Grupo
        /// </summary>
        /// <param name="papel"></param>
        /// <returns>Id do PapelGrupo</returns>
        public int Create(Papelgrupo papel)
        {
            _context.Papelgrupos.Add(papel);
            _context.SaveChanges();

            return papel.IdPapelGrupo;
        }
         /// <summary>
        /// Método que deleta um Papel Grupo
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var papel = _context.Papelgrupos.Find(id);
            _context.Remove(papel);
            _context.SaveChanges();
        }
        /// <summary>
        /// Metodo usado para editar um Papel Grupo
        /// </summary>
        /// <param name="papel"></param>
        public void Edit(Papelgrupo papel)
        {
            _context.Update(papel);
            _context.SaveChanges();
        }
        /// <summary>
        /// Consulta um Papel Grupo no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Palpel Grupo correspondente ao id ou um Papel Grupo vazio</returns>
        public Papelgrupo Get(int id)
        {
            return _context.Papelgrupos.Find(id);
        }
        /// <summary>
        /// Consulta todos os Papel Grupo no banco de dados
        /// </summary>
        /// <returns>Uma lista contendo todos os Papel Grupo</returns>
        public IEnumerable<Papelgrupo> GetAll()
        {
            return _context.Papelgrupos.AsNoTracking();
        }
    }
}
