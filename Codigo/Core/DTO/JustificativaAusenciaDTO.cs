using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

/// <summary>
/// Dados enviados pelo associado ao justificar a ausência em um ensaio ou evento.
/// </summary>
public class JustificativaAusenciaDTO
{
    public int IdEnsaio { get; set; }

    public int IdEvento { get; set; }

    [Required(ErrorMessage = "A justificativa é obrigatória.")]
    [StringLength(200, ErrorMessage = "A justificativa deve ter no máximo 200 caracteres.")]
    public string? Justificativa { get; set; }
}
