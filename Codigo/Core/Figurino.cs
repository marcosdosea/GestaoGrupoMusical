using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Figurino
    {
        public Figurino()
        {
            Figurinomanequims = new HashSet<Figurinomanequim>();
            Movimentacaofigurinos = new HashSet<Movimentacaofigurino>();
            IdApresentacaos = new HashSet<Evento>();
            IdEnsaios = new HashSet<Ensaio>();
        }

        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime? Data { get; set; }
        public int IdGrupoMusical { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual ICollection<Figurinomanequim> Figurinomanequims { get; set; }
        public virtual ICollection<Movimentacaofigurino> Movimentacaofigurinos { get; set; }

        public virtual ICollection<Evento> IdApresentacaos { get; set; }
        public virtual ICollection<Ensaio> IdEnsaios { get; set; }
    }
}
