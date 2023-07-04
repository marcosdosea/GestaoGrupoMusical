using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IInformativoService
    {
        Task<bool> Create(Informativo informativo);
        Task<bool> Edit(Informativo informativo);
        Task<bool> Delete(int idPessoa, int idGrupoMusical);
        Task<Informativo> Get(int idPessoa, int idGrupoMusical);
        Task<IEnumerable<Informativo>> GetAll();
    }
}
