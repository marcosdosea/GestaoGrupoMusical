using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    internal interface IPessoaService
    {
        int Create(Pessoa cidadao);
        void Edit(Pessoa cidadao);
        void Delete(int id);
        Pessoa Get(int id);
        IEnumerable<Pessoa> GetAll();
    }
}
