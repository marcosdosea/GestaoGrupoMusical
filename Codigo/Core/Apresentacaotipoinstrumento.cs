using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Apresentacaotipoinstrumento
    {
        public int IdApresentacao { get; set; }
        public int IdTipoInstrumento { get; set; }
        public int QuantidadePlanejada { get; set; }
        public int QuantidadeConfirmada { get; set; }

        public virtual Evento IdApresentacaoNavigation { get; set; } = null!;
        public virtual Tipoinstrumento IdTipoInstrumentoNavigation { get; set; } = null!;
    }
}
