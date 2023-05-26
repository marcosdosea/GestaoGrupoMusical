using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Manequim
    {
        public Manequim()
        {
            Figurinomanequims = new HashSet<Figurinomanequim>();
            Movimentacaofigurinos = new HashSet<Movimentacaofigurino>();
            Pessoas = new HashSet<Pessoa>();
        }

        public int Id { get; set; }
        public string Tamanho { get; set; } = null!;
        public string Descricao { get; set; } = null!;

        public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; }
        public virtual ICollection<Movimentacaofigurino> Movimentacaofigurinos { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
