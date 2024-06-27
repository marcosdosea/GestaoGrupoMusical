using System;
using System.Collections.Generic;

namespace Core;

public partial class Receitafinanceirapessoa
{
    public int IdReceitaFinanceira { get; set; }

    public int IdPessoa { get; set; }

    public decimal Valor { get; set; }

    public decimal ValorPago { get; set; }

    public DateTime DataPagamento { get; set; }

    public string? Observacoes { get; set; }

    public string Status { get; set; } = null!;

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual Receitafinanceira IdReceitaFinanceiraNavigation { get; set; } = null!;
}
