using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Papelgrupo
    {
        public Papelgrupo()
        {
            Ensaiopessoas = new HashSet<Ensaiopessoa>();
            Eventopessoas = new HashSet<Eventopessoa>();
            Pessoas = new HashSet<Pessoa>();
        }

        public int IdPapelGrupo { get; set; }
        public string Nome { get; set; } = null!;

        public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; }
        public virtual ICollection<Eventopessoa> Eventopessoas { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}
