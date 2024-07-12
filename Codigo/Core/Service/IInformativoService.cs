using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IInformativoService
    {
        Task<HttpStatusCode> Create(Informativo informativo);
        HttpStatusCode Edit(Informativo informativo);
        Task <HttpStatusCode> Delete(uint id);
        Task<Informativo?> Get(uint id);
        Task<IEnumerable<Informativo>> GetAll();              
        Task<IEnumerable<Informativo>> GetAllInformativoServiceIdGrupo(int idGrupoMusical, int IdPessoa);
        Task<HttpStatusCode> NotificarInformativoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, uint idInformativo, string mensagem);
    }
}
