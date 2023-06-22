using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IManequim
    {
        int Create(Manequim manequim);
        void Edit(Manequim manequim);
        void Delete(int id);
        Manequim Get(int id);
        IEnumerable<Manequim> GetAll();
    }
}
