using System;
using System.Collections.Generic;

namespace Core;

public partial class Tipoinstrumento
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Apresentacaotipoinstrumento> Apresentacaotipoinstrumentos { get; set; } = new List<Apresentacaotipoinstrumento>();

    public virtual ICollection<Eventopessoa> Eventopessoas { get; set; } = new List<Eventopessoa>();

    public virtual ICollection<Instrumentomusical> Instrumentomusicals { get; set; } = new List<Instrumentomusical>();

    public virtual ICollection<Pessoa> IdPessoas { get; set; } = new List<Pessoa>();
}
