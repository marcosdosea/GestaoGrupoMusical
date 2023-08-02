using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class FigurinoDTO
    {

        public int IdFigurino { get; set; }
        public int IdManequim { get; set; }
        public int IdGrupoMusical { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime? Data { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadeEntregue { get; set; }

    }
}
