using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AssociadoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public sbyte Ativo { get; set; }
    }
}
