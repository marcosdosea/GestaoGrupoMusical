using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class FrequenciaEnsaioDTO
    {
        public int Id { get; set; }
        public int IdGrupoMusical { get; set; }

        public DateTime? DataHoraInicio { get; set; }

        public DateTime? DataHoraFim { get; set; }

        public Tipo Tipo { get; set; }

        public string Regentes { get; set; } = string.Empty;

        public string? Figurino { get; set; }

        public List<AssociadoDTO>? AssociadosDTO { get; set; }

        public string? Local { get; set; }
    }

    public enum Tipo
    {
        FIXO,
        EXTRA
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
