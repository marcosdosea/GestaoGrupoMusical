using System;
using System.Collections.Generic;

namespace Core;

public partial class Pessoatipoinstrumento
{
    public int IdPessoa { get; set; }

    public int IdTipoInstrumento { get; set; }

    public int PrioridadeEscolha { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual Tipoinstrumento IdTipoInstrumentoNavigation { get; set; } = null!;
}
