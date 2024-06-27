using System;
using System.Collections.Generic;

namespace Core;

public partial class Materialestudo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Link { get; set; } = null!;

    public DateTime Data { get; set; }

    public int IdGrupoMusical { get; set; }

    public int IdColaborador { get; set; }

    public virtual Pessoa IdColaboradorNavigation { get; set; } = null!;

    public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
}
