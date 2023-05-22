using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Papelgrupo
    {
        public Papelgrupo()
        {
            Pessoas = new HashSet<Pessoa>();
        }

        public int IdPapelGrupo { get; set; }
        public string Nome { get; set; } = null!;

        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
