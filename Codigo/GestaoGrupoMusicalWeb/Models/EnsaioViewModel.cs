namespace GestaoGrupoMusicalWeb.Models
{
    public class EnsaioViewModel
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
        public int IdRegente { get; set; }
    }
}
