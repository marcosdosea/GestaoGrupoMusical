using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Figurino
    {
        public Figurino()
        {
            Entregarfigurinos = new HashSet<Entregarfigurino>();
            Figurinomanequims = new HashSet<Figurinomanequim>();
            IdApresentacaos = new HashSet<Apresentacao>();
            IdEnsaios = new HashSet<Ensaio>();
        }

        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime? Data { get; set; }
        public int IdGrupoMusical { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual ICollection<Entregarfigurino> Entregarfigurinos { get; set; }
        public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; }

        public virtual ICollection<Apresentacao> IdApresentacaos { get; set; }
        public virtual ICollection<Ensaio> IdEnsaios { get; set; }
    }
}
