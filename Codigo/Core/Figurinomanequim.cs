using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Figurinomanequim
    {
        public int IdFigurino { get; set; }
        public int IdManequim { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadeEntregue { get; set; }
        public int QuantidadeDescartada { get; set; }

        public virtual Figurino IdFigurinoNavigation { get; set; } = null!;
        public virtual Manequim IdManequimNavigation { get; set; } = null!;
    }
}
