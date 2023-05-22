using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Movimentacaoinstrumento
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int IdInstrumentoMusical { get; set; }
        public int IdAssociado { get; set; }
        public int IdColaborador { get; set; }
        public sbyte ConfirmacaoAssociado { get; set; }
        public string TipoMovimento { get; set; } = null!;

        public virtual Pessoa IdAssociadoNavigation { get; set; } = null!;
        public virtual Pessoa IdColaboradorNavigation { get; set; } = null!;
        public virtual Instrumentomusical IdInstrumentoMusicalNavigation { get; set; } = null!;
    }
}
