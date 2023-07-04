namespace GestaoGrupoMusicalWeb.Models
{
    public class EnsaioViewModelIndexDTO
    {
        public int Id { get; set; }
        public DateTime? DataHoraInicio { get; set; }
        public string Tipo { get; set; } = null!;
        public string? Local { get; set; }
        public sbyte PresencaObrigatoria { get; set; }
    }
}
