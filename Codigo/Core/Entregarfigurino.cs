using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Entregarfigurino
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int IdFigurino { get; set; }
        public int IdManequim { get; set; }
        public int IdAssociado { get; set; }
        public int IdColaborador { get; set; }
        public string Status { get; set; } = null!;
        public sbyte ConfirmacaoAssociado { get; set; }

        public virtual Pessoa IdAssociadoNavigation { get; set; } = null!;
        public virtual Pessoa IdColaboradorNavigation { get; set; } = null!;
        public virtual Figurino IdFigurinoNavigation { get; set; } = null!;
        public virtual Manequim IdManequimNavigation { get; set; } = null!;
    }
}
