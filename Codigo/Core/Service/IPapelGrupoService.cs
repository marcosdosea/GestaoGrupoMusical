using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IPapelGrupoService
    {
        int Create(Papelgrupo papel);
        void Edit(Papelgrupo papel);
        void Delete(int id);
        Papelgrupo Get(int id);
        IEnumerable<Papelgrupo> GetAll();
    }
}
