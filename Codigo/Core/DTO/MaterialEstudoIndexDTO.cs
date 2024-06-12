using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class MaterialEstudoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Link { get; set; } = null!;
        public string Data { get; set; } = null!;
        public int IdGrupoMusical { get; set; }
    }
}
