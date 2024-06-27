using System;
using System.Collections.Generic;

namespace Core;

public partial class Grupomusical
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string RazaoSocial { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string? Cep { get; set; }

    public string? Rua { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string Estado { get; set; } = null!;

    public string? Pais { get; set; }

    public string? Email { get; set; }

    public string? Youtube { get; set; }

    public string? Instagram { get; set; }

    public string? Facebook { get; set; }

    public string? Telefone1 { get; set; }

    public string? Telefone2 { get; set; }

    public string? Banco { get; set; }

    public string? Agencia { get; set; }

    public string? NumeroContaBanco { get; set; }

    public string? ChavePix { get; set; }

    public string? ChavePixtipo { get; set; }

    public virtual ICollection<Ensaio> Ensaios { get; set; } = new List<Ensaio>();

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual ICollection<Figurino> Figurinos { get; set; } = new List<Figurino>();

    public virtual ICollection<Informativo> Informativos { get; set; } = new List<Informativo>();

    public virtual ICollection<Instrumentomusical> Instrumentomusicals { get; set; } = new List<Instrumentomusical>();

    public virtual ICollection<Materialestudo> Materialestudos { get; set; } = new List<Materialestudo>();

    public virtual ICollection<Pessoa> Pessoas { get; set; } = new List<Pessoa>();

    public virtual ICollection<Receitafinanceira> Receitafinanceiras { get; set; } = new List<Receitafinanceira>();
}
