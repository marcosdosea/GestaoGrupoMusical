using Core;
using Core.Datatables;
using Core.DTO;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IInformativoService
    {
        Task<HttpStatusCode> Create(Informativo informativo);
        HttpStatusCode Edit(Informativo informativo);
        Task<HttpStatusCode> Delete(uint id);
        Task<Informativo?> Get(uint id); // <-- Apenas este Get com um parâmetro
        Task<IEnumerable<Informativo>> GetAll();
        Task<IEnumerable<Informativo>> GetAllInformativoServiceIdGrupo(int idGrupoMusical, int idPessoa);
        IEnumerable<Informativo> GetAllInformativoServicePorIdGrupoMusical(int idGrupoMusical);
        Task<HttpStatusCode> NotificarInformativoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, uint idInformativo, string mensagem);
        DatatableResponse<InformativoIndexDTO> GetDataPage(DatatableRequest request, IEnumerable<InformativoIndexDTO> listaInformativoDTO);
    }
}