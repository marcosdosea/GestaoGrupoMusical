using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
        public async Task<HttpStatusCode> Create(Grupomusical grupomusical)
        {
            try
            {
                await _context.Grupomusicals.AddAsync(grupomusical);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {

                return HttpStatusCode.InternalServerError;
            }
        }
        /// <summary>
        /// Metodo usado para deletar um grupo musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        public async Task<HttpStatusCode> Delete(int id)
        {
            var grupo = await _context.Grupomusicals.FindAsync(id);

            try
            {
                _context.Remove(grupo);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
        /// <summary>
        /// Metodo usado para editar um grupo musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        public async Task<HttpStatusCode> Edit(Grupomusical grupomusical)
        {
            try
            {
                _context.Update(grupomusical);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
        /// <summary>
        /// Pegar um Grupo Musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Retorna 1 grupo musical</returns>
        public async Task<Grupomusical> Get(int id)
        {
            return await _context.Grupomusicals.FindAsync(id);
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

        public async Task<int> GetIdGrupo(string cpf)
        {
            var query = await _context.Pessoas
                 .Where(g => g.Cpf == cpf)
                 .Select(g => g.IdGrupoMusical).FirstOrDefaultAsync();
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
                              Data = pessoa.DataEntrada.Value.ToString("dd/MM/yyyy"),
                              Papel = pessoa.IdPapelGrupoNavigation.Nome
                          }
                ).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<IEnumerable<Papelgrupo>> GetPapeis()
        {
            var query = await (from papel in _context.Papelgrupos
                         where papel.Nome.ToUpper() == "COLABORADOR" || papel.Nome.ToUpper() == "REGENTE"
                         select papel
                         ).AsNoTracking().ToListAsync();

            return query;
        }
    }
}
