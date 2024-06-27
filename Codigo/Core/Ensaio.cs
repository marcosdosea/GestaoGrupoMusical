using System;
using System.Collections.Generic;

namespace Core;

public partial class Ensaio
{
    public int Id { get; set; }

    public int IdGrupoMusical { get; set; }

    public string Tipo { get; set; } = null!;

    public DateTime DataHoraInicio { get; set; }

    public DateTime DataHoraFim { get; set; }

    public sbyte PresencaObrigatoria { get; set; }

    public string? Local { get; set; }

    public string? Repertorio { get; set; }

    public int IdColaboradorResponsavel { get; set; }

    public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; } = new List<Ensaiopessoa>();

    public virtual Pessoa IdColaboradorResponsavelNavigation { get; set; } = null!;

    public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;

    public virtual ICollection<Figurino> IdFigurinos { get; set; } = new List<Figurino>();
}
