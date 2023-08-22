using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Ensaiopessoa
    {
        public int IdPessoa { get; set; }
        public int IdEnsaio { get; set; }
        public sbyte Presente { get; set; }
        public string? JustificativaFalta { get; set; }
        public sbyte JustificativaAceita { get; set; }
        public int IdPapelGrupoPapelGrupo { get; set; }

        public virtual Ensaio IdEnsaioNavigation { get; set; } = null!;
        public virtual Papelgrupo IdPapelGrupoPapelGrupoNavigation { get; set; } = null!;
        public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
    }
}
