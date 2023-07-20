using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class InstrumentoAssociadoDTO
    {

        public class MovimentacaoAssociado
        {
            public int Id { get; set; }

            [Display(Name = "Instrumento")]
            public string NomeInstrumento { get; set; } = string.Empty;

            public DateTime Data { get; set; }

            [Display(Name = "Status")]
            public string NomeStatus { get; set; } = string.Empty;

            public bool Status { get; set; }
        }

        public class MovimentacoesAssociado
        {
            public IEnumerable<MovimentacaoAssociado>? Emprestimos { get; set; }

            public IEnumerable<MovimentacaoAssociado>? Devolucoes { get; set; }
        }
    }
}
