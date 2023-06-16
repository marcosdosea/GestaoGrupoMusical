
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class InstrumentoMusicalDTO
    {
        public int Id { get; set; }

        [Display(Name = "Patrimônio")]
        public string Patrimonio { get; set; } = string.Empty;

        [Display(Name = "Instrumento")]
        public string NomeInstrumento { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        [Display (Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        public Dictionary<string, string> EnumStatus { get; } = new()
        {
            { "DISPONIVEL", "Disponível" },
            { "EMPRESTADO", "Emprestado" },
            { "DANIFICADO" ,"Danificado" }
        };
    }
}
