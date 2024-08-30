using Core.Datatables;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IFinanceiroService
    {
        Task<IEnumerable<FinanceiroIndexDataPage>> GetAllFinanceiroPorIdGrupo(int idGrupoMusical, int mesesAtrasados);
        DatatableResponse<FinanceiroIndexDataPage> GetDataPage(DatatableRequest request, IEnumerable<FinanceiroIndexDataPage> materialEstudoIndexDTO);
    }
}
