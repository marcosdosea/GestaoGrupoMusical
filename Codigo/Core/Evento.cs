using System;
using System.Collections.Generic;

namespace Core;

public partial class Evento
{
    public int Id { get; set; }

    public int IdGrupoMusical { get; set; }

    public DateTime DataHoraInicio { get; set; }

    public DateTime DataHoraFim { get; set; }

    public string? Local { get; set; }

    public string? Repertorio { get; set; }

    public int IdColaboradorResponsavel { get; set; }

    public virtual ICollection<Apresentacaotipoinstrumento> Apresentacaotipoinstrumentos { get; set; } = new List<Apresentacaotipoinstrumento>();

    public virtual ICollection<Eventopessoa> Eventopessoas { get; set; } = new List<Eventopessoa>();

    public virtual Pessoa IdColaboradorResponsavelNavigation { get; set; } = null!;

    public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;

    public virtual ICollection<Figurino> IdFigurinos { get; set; } = new List<Figurino>();
}
