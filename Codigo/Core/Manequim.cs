using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Manequim
    {
        public Manequim()
        {
            Entregarfigurinos = new HashSet<Entregarfigurino>();
            Figurinomanequims = new HashSet<Figurinomanequim>();
            Pessoas = new HashSet<Pessoa>();
        }

        public int Id { get; set; }
        public string Tamanho { get; set; } = null!;
        public string Descricao { get; set; } = null!;

        public virtual ICollection<Entregarfigurino> Entregarfigurinos { get; set; }
        public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
