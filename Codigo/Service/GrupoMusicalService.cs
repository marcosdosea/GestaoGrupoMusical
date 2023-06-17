using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class GrupoMusicalService : IGrupoMusicalService
    {

        private readonly GrupoMusicalContext _context;

        public GrupoMusicalService(GrupoMusicalContext context)
        {
            _context = context;
        }

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
        /// <summary>
        /// Pegar um Grupo Musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Retorna 1 grupo musical</returns>
        public Grupomusical Get(int id)
        {
            return _context.Grupomusicals.Find(id);
        }

        /// <summary>
        /// Pega todos os grupos musicais
        /// </summary>
        /// <returns>Uma lista de grupo musicais</returns>
        public IEnumerable<Grupomusical> GetAll()
        {
            return _context.Grupomusicals.AsNoTracking();
        }
        /// <summary>
        /// DTO de grupo musicais
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<GrupoMusicalDTO> GetAllDTO()
        {
            var query = _context.Grupomusicals
                .OrderBy(g => g.Id)
                .Select(g =>
                    new GrupoMusicalDTO
                    {
                        Id = g.Id,
                        Name = g.Nome
                    }) ;
            return query.AsNoTracking();
        }
    }
}
