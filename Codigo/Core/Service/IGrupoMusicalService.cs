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
        int Create(Grupomusical grupomusical);
        void Edit(Grupomusical grupomusical);
        void Delete(int id);
        Grupomusical Get(int id);
        IEnumerable<Grupomusical> GetAll();
        IEnumerable<GrupoMusicalDTO> GetAllDTO();
    }
}
