using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Ensaio
    {
        public Ensaio()
        {
            Ensaiopessoas = new HashSet<Ensaiopessoa>();
            IdFigurinos = new HashSet<Figurino>();
        }

        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }
        public string Tipo { get; set; } = null!;
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public sbyte PresencaObrigatoria { get; set; }
        public string? Local { get; set; }
        public string? Repertorio { get; set; }
        public int IdColaboradorResponsavel { get; set; }
        public int IdRegente { get; set; }

        public virtual Pessoa IdColaboradorResponsavelNavigation { get; set; } = null!;
        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual Pessoa IdRegenteNavigation { get; set; } = null!;
        public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; }

        public virtual ICollection<Figurino> IdFigurinos { get; set; }
    }
}
