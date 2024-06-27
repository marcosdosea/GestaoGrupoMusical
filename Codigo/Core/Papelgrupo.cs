using System;
using System.Collections.Generic;

namespace Core;

public partial class Papelgrupo
{
    public int IdPapelGrupo { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; } = new List<Ensaiopessoa>();

    public virtual ICollection<Eventopessoa> Eventopessoas { get; set; } = new List<Eventopessoa>();

    public virtual ICollection<Pessoa> Pessoas { get; set; } = new List<Pessoa>();
}
