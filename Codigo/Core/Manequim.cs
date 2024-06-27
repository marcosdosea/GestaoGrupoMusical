using System;
using System.Collections.Generic;

namespace Core;

public partial class Manequim
{
    public int Id { get; set; }

    public string Tamanho { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; } = new List<Figurinomanequim>();

    public virtual ICollection<Movimentacaofigurino> Movimentacaofigurinos { get; set; } = new List<Movimentacaofigurino>();

    public virtual ICollection<Pessoa> Pessoas { get; set; } = new List<Pessoa>();
}
