using Core.Datatables;
using Core.DTO;
using System.Collections;
using System.Net;

namespace Core.Service
{
    public interface IMaterialEstudoService
    {
        Task<bool> Create(Materialestudo materialEstudo);

        Task<bool> Edit(Materialestudo materialEstudo);

        Task<HttpStatusCode> Delete(int id);
        Task<Materialestudo?> Get(int id);
        Task<IEnumerable<Materialestudo>> GetAll();
        Task<IEnumerable<Materialestudo>> GetAllMaterialEstudoPerIdGrupo(int idGrupoMusical);
        Task<DatatableResponse<Materialestudo>> GetDataPage(DatatableRequest request, int idGrupo);
        Task<HttpStatusCode> NotificarMaterialViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idMaterialEstudo);
    }
}
