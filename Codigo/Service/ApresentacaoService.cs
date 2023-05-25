using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class ApresentacaoService : IApresentacao
    {
        private readonly GrupoMusicalContext _context;

        public ApresentacaoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método usado a para adicionar uma nova apresentação
        /// </summary>
        /// <param name="apresentacao"></param>
        /// <returns>Id do Grupo Musical</returns>
        public int Create(Apresentacao apresentacao)
        {
            _context.Add(apresentacao);
            _context.SaveChanges();
            return apresentacao.Id;
        }
        /// <summary>
        /// Método que deleta uma apresentação 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var apresentacao = _context.Apresentacaos.Find(id);
            _context.Remove(apresentacao);
            _context.SaveChanges();
        }

        /// <summary>
        /// Metodo usado para editar um Grupo Musical
        /// </summary>
        /// <param name="apresentacao"></param>
        public void Edit(Apresentacao apresentacao)
        {
            _context.Update(apresentacao);
            _context.SaveChanges();

        }

        public Apresentacao Get(int id)
        {
            return _context.Apresentacaos.Find(id);
        }

        public IEnumerable<Apresentacao> GetAll()
        {
            return _context.Apresentacaos.AsNoTracking();
        }
    }
}
