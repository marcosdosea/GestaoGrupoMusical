using Core.Datatables;
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
        IEnumerable<Informativo> GetAllInformativoServicePorIdGrupoMusical(int idGrupoMusical);
        DatatableResponse<InformativoIndexDTO> GetDataPage(DatatableRequest request, IEnumerable<InformativoIndexDTO> InformativoIndexDTO);
        Task<HttpStatusCode> NotificarInformativoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, uint idInformativo, string mensagem);
    }
}
