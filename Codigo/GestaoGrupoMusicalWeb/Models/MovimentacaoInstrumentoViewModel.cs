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
        public int IdAssociado { get; set; }

        [Display(Name = "Colaborador")]
        //TODO: Remover essa propriedade apos autenticacao
        public int IdColaborador { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;

        [Display(Name = "Movimentação")]
        public string Movimentacao { get; set; } = string.Empty;



        public Dictionary<string, string> MovimentacaoEnum { get; } = new()
        {
            { "Empréstimo", "EMPRESTIMO" },
            { "Devolução", "DEVOLUCAO" }
        };

        public SelectList? ListaAssociado { get; set; }
    }
}
