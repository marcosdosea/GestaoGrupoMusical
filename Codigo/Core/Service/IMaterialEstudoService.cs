using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IMaterialEstudoService
    {
        Task<bool> Create(Materialestudo materialEstudo);

        Task<bool> Edit(Materialestudo materialEstudo);

        Task<bool> Delete(int id);
        Task<Materialestudo?> Get(int id);
        Task<IEnumerable<Materialestudo?>> GetAll();
    }
}
