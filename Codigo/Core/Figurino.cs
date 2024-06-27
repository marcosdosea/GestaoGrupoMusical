using System;
using System.Collections.Generic;

namespace Core;

public partial class Figurino
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime? Data { get; set; }

    public int IdGrupoMusical { get; set; }

    public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; } = new List<Figurinomanequim>();

    public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;

    public virtual ICollection<Movimentacaofigurino> Movimentacaofigurinos { get; set; } = new List<Movimentacaofigurino>();

    public virtual ICollection<Evento> IdApresentacaos { get; set; } = new List<Evento>();

    public virtual ICollection<Ensaio> IdEnsaios { get; set; } = new List<Ensaio>();
}
