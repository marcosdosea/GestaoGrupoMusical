using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IFigurinoService
    {
        Task<int> Create(FigurinoDTO figurino);
        Task<int> Edit(FigurinoDTO figurino);
        Task<int> Delete(int id);
        FigurinoDTO Get(int id);
        Task<IEnumerable<FigurinoDTO>> GetAll();
    }
}
