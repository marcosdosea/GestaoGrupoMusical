using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace GestaoGrupoMusicalWeb.Models
{
    public class MovimentacaoFigurino
    {

        public int Id { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string DataString { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");


        [Display(Name = "Figurino")]
        public string NomeFigurino { get; set; } = string.Empty;

        public int IdFigurino { get; set; }

        [Display(Name = "Tamanho")]
        [Required(ErrorMessage = "O campo tamanho é obrigatório")]
        public int IdManequim { get; set; }

        [Display(Name = "Associado")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdAssociado { get; set; }

        public int IdColaborador { get; set; }

        public string Status { get; set; } = null!;

        public SelectList? ListaManequim { get; set; }
        public SelectList? ListaAssociado { get; set; }

        [Display(Name = "Movimentação")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Movimentacao { get; set; } = "EMPRESTIMO";

        public IEnumerable<MovimentacaoInstrumentoDTO>? Movimentacoes { get; set; }

        public Dictionary<string, string> MovimentacaoEnum { get; } = new()
        {
            { "Empréstimo", "EMPRESTIMO" },
            { "Devolução", "DEVOLUCAO" }
        };

    }
}
