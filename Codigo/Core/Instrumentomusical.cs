using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Instrumentomusical : IInstrumentoMusicalService
    {
        public Instrumentomusical()
        {
            Movimentacaoinstrumentos = new HashSet<Movimentacaoinstrumento>();
        }

        public int Id { get; set; }
        public string Patrimonio { get; set; } = null!;
        public DateTime DataAquisicao { get; set; }
        public string Status { get; set; } = null!;
        public int IdTipoInstrumento { get; set; }
        public int IdGrupoMusical { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual Tipoinstrumento IdTipoInstrumentoNavigation { get; set; } = null!;
        public virtual ICollection<Movimentacaoinstrumento> Movimentacaoinstrumentos { get; set; }
    }
}
