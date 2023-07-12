using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Identity;
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
        public async Task<int> Create(Grupomusical grupomusical)
        {


                try
                {
                    await _context.Grupomusicals.AddAsync(grupomusical);
                    await _context.SaveChangesAsync();

                    return 200;
                }catch (Exception ex)
                {

                    return 500;
                }
        }
        /// <summary>
        /// Metodo para deletar o Grupo Musical
        /// </summary>
        /// <param name="id"></param>
         public async Task<int> Delete(int id)
        {

            var grupo = await _context.Grupomusicals.FindAsync(id);
            using ( var transaction = _context.Database.BeginTransaction())
                try
                {
                    
                    _context.Remove(grupo);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return 200;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return 500;
                }

        }
        /// <summary>
        /// Metodo usado para editar um Grupo Musical
        /// </summary>
        /// <param name="grupomusical"></param>
        public async Task<int> Edit(Grupomusical grupomusical)
        {

                try
                {
                    _context.Update(grupomusical);
                    await _context.SaveChangesAsync();

                    return 200;
                }
                catch
                {

                    return 500;
                }
                
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
