using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class EnsaioService : IEnsaio
    {
        private readonly GrupoMusicalContext _context;
        public EnsaioService(GrupoMusicalContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Cadastra um novo Ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se cadastrou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<bool> Create(Ensaio ensaio)
        {
            try
            {
                await _context.Ensaios.AddAsync(ensaio);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Deleta um Ensaio do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se deletou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<bool> Delete(int id)
        {
            try
            {
                _context.Ensaios.Remove(await Get(id));
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Edita um Ensaio do banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se deletou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<bool> Edit(Ensaio ensaio)
        {
            try
            {
                _context.Ensaios.Update(ensaio);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Consulta um Ensaio no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Ensaio correspondente ao id ou um Ensaio vazio</returns>
        public async Task<Ensaio> Get(int id)
        {
            return await _context.Ensaios.FindAsync(id) ?? new Ensaio();
        }
        /// <summary>
        /// Consulta todos os Ensaios no banco de dados
        /// </summary>
        /// <returns>Uma lista contendo todos os Ensaios</returns>
        public IAsyncEnumerable<Ensaio> GetAll()
        {
            return  _context.Ensaios.AsNoTracking().AsAsyncEnumerable();
        }
    }
}
