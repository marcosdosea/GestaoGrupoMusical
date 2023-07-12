using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IGrupoMusicalService
    {
        Task<int> Create (Grupomusical grupomusical);
        Task<int> Edit(Grupomusical grupomusical);
        Task<int> Delete(int id);
        Task<Grupomusical> Get(int id);
        Task<IEnumerable<Grupomusical>> GetAll();
        Task<IEnumerable<GrupoMusicalDTO>> GetAllDTO();
    }
}
