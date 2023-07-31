using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class MovimentacaoInstrumentoViewModel
    {
        [Display(Name = "Patrimônio")]
        public string Patrimonio { get; set; } = string.Empty;

        [Display(Name = "Instrumento")]
        public string NomeInstrumento { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdAssociado { get; set; }

        public int IdInstrumentoMusical { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        public string DataString { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");

        [Display(Name = "Movimentação")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Movimentacao { get; set; } = "EMPRESTIMO";

        public IEnumerable<MovimentacaoInstrumentoDTO>? Movimentacoes { get; set; }

        public Dictionary<string, string> MovimentacaoEnum { get; } = new()
        {
            { "Empréstimo", "EMPRESTIMO" },
            { "Devolução", "DEVOLUCAO" }
        };

        public SelectList? ListaAssociado { get; set; }
    }
}
