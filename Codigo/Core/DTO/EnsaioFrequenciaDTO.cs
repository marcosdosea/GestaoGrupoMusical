using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class EnsaioFrequenciaDTO
    {
        [Display(Name = "Início")]
        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public IEnumerable<string>? Regentes { get; set; }

        public string? Local { get; set; }
        public IEnumerable<EnsaioListaFrequenciaDTO>? Frequencias { get; set; }
    }

    public class EnsaioListaFrequenciaDTO
    {
        public int IdEnsaio { get; set; }

        public int IdPessoa { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        [Display(Name = "Justificativa Ausência")]
        public string? Justificativa { get; set; }

        public bool Presente { get; set; }

        [Display(Name = "Justificativa Aceita")]
        public bool JustificativaAceita { get; set; }
    }
}
