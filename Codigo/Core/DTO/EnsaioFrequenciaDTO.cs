namespace Core.DTO
{
    public class EnsaioFrequenciaDTO
    {
        public string Cpf { get; set; } = string.Empty;

        public string NomeAssociado { get; set; } = string.Empty;

        public string Justificativa { get; set; } = string.Empty;

        public bool Presente { get; set; }

        public bool JustificativaAceita { get; set; }
    }
}
