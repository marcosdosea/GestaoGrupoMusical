using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Informativo
    {
        public uint Id { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }
        public string Mensagem { get; set; } = null!;
        public DateTime Data { get; set; }
        public sbyte EntregarAssociadosAtivos { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
    }
}
