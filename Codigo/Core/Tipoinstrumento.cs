using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Tipoinstrumento
    {
        public Tipoinstrumento()
        {
            Apresentacaopessoas = new HashSet<Apresentacaopessoa>();
            Apresentacaotipoinstrumentos = new HashSet<Apresentacaotipoinstrumento>();
            Instrumentomusicals = new HashSet<Instrumentomusical>();
            IdPessoas = new HashSet<Pessoa>();
        }

        public int Id { get; set; }
        public string Nome { get; set; } = null!;

        public virtual ICollection<Apresentacaopessoa> Apresentacaopessoas { get; set; }
        public virtual ICollection<Apresentacaotipoinstrumento> Apresentacaotipoinstrumentos { get; set; }
        public virtual ICollection<Instrumentomusical> Instrumentomusicals { get; set; }

        public virtual ICollection<Pessoa> IdPessoas { get; set; }
    }
}
