using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IInformativoService
    {
        Task<HttpStatusCode> Create(Informativo informativo);
        Task<bool> Edit(Informativo informativo);
        Task<HttpStatusCode> Delete(int idPessoa, int idGrupoMusical);
        Task<Informativo?> Get(int idPessoa, int idGrupoMusical);
        Task<IEnumerable<Informativo>> GetAll();              
        Task<IEnumerable<Informativo>> GetAllInformativoServiceIdGrupo(int idGrupoMusical, int IdPessoa);
    }
}
