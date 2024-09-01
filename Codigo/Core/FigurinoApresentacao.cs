using System;
using System.Collections.Generic;

namespace Core;

public partial class FigurinoApresentacao
{
    public int IdFigurino { get; set; }

    public int IdApresentacao { get; set; }    

    public virtual Evento IdApresentacaoEvento { get; set; } = null!;

    public virtual Figurino IdManequimNIdFigurinoEventoavigation { get; set; } = null!;
}
