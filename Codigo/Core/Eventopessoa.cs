using System;
using System.Collections.Generic;

namespace Core;

public partial class Eventopessoa
{
    public int IdEvento { get; set; }

    public int IdPessoa { get; set; }

    public int IdTipoInstrumento { get; set; }

    public sbyte Presente { get; set; }

    public string? JustificativaFalta { get; set; }

    public sbyte JustificativaAceita { get; set; }

    public string Status { get; set; } = null!;

    public int IdPapelGrupoPapelGrupo { get; set; }

    public virtual Evento IdEventoNavigation { get; set; } = null!;

    public virtual Papelgrupo IdPapelGrupoPapelGrupoNavigation { get; set; } = null!;

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual Tipoinstrumento IdTipoInstrumentoNavigation { get; set; } = null!;
}
