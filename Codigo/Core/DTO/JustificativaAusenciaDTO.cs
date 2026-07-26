namespace Core.DTO
{
    public class JustificarAusenciaEnsaioDTO
    {
        public int IdEnsaio { get; set; }
        public string? Justificativa { get; set; }
    }

    public class JustificarAusenciaEventoDTO
    {
        public int IdEvento { get; set; }
        public string? Justificativa { get; set; }
    }
}

