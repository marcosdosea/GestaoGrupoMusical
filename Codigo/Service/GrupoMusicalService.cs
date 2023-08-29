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
        /// Metodo usado para adicionar o Grupo Musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        public async Task<int> Create(Grupomusical grupomusical)
        {


            try
            {
                await _context.Grupomusicals.AddAsync(grupomusical);
                await _context.SaveChangesAsync();

                return 200;
            }
            catch (Exception ex)
            {

                return 500;
            }
        }
        /// <summary>
        /// Metodo usado para deletar um grupo musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        public async Task<int> Delete(int id)
        {

            var grupo = await _context.Grupomusicals.FindAsync(id);

            try
            {

                _context.Remove(grupo);
                await _context.SaveChangesAsync();
                return 200;
            }
            catch (Exception ex)
            {
                return 500;
            }

        }
        /// <summary>
        /// Metodo usado para editar um grupo musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
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
                    });
            return query.AsNoTracking();
        }

        public int GetIdGrupo(string cpf)
        {
            var query = _context.Pessoas
                 .Where(g => g.Cpf == cpf)
                 .Select(g => g.IdGrupoMusical).FirstOrDefault();
            return query;
        }

        public bool GetCNPJExistente(int id, string cnpj)
        {
            var query =   _context.Set<Grupomusical>().AsNoTracking().FirstOrDefault( p => p.Id == id && p.Cnpj == cnpj);
            if(query != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ColaboradoresDTO>> GetAllColaboradores(int idGrupo)
        {
            int idColaborador = (await _context.Papelgrupos
                                .Where(p => p.Nome.ToUpper() == "COLABORADOR")
                                .AsNoTracking()
                                .FirstOrDefaultAsync())
                                ?.IdPapelGrupo ?? 0;

            int idRegente = (await _context.Papelgrupos
                            .Where(p => p.Nome.ToUpper() == "REGENTE")
                            .AsNoTracking()
                            .FirstOrDefaultAsync())
                            ?.IdPapelGrupo ?? 0;

            var query = await ( from pessoa in _context.Pessoas
                          where pessoa.IdGrupoMusical == idGrupo &&
                                (pessoa.IdPapelGrupo == idRegente || pessoa.IdPapelGrupo == idColaborador)
                          select new ColaboradoresDTO
                          {
                              Id = pessoa.Id,
                              Cpf = pessoa.Cpf,
                              Nome = pessoa.Nome,
                              Data = pessoa.DataEntrada,
                              Papel = pessoa.IdPapelGrupoNavigation.Nome
                          }
                ).AsNoTracking().ToListAsync();

            return query;
        }
    }
}
