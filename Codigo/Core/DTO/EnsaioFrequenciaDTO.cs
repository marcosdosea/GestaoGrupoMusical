using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class EnsaioFrequenciaDTO
    {
        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        [Display (Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public bool Presente { get; set; }
        [Display(Name = "Justificativa Aceita")]
        public bool JustificativaAceita { get; set; }
    }
}
