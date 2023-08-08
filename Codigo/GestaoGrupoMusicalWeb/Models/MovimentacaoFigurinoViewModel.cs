using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlX.XDevAPI.Relational;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace GestaoGrupoMusicalWeb.Models
{
    public class MovimentacaoFigurinoViewModel
    {

        public int Id { get; set; }

        public string DataFigurinoString { get; set; }

        //data de emprestimo/devolução do figurino
        public DateTime Data { get; set; } = DateTime.Today;
        //============================================================================

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
        public string Movimentacao { get; set; } = "ENTREGUE";

        public IEnumerable<MovimentacaoFigurinoDTO>? Movimentacoes { get; set; }

        public Boolean Danificado { get; set; }

        public Dictionary<string, string> MovimentacaoEnum { get; } = new()
        {
            { "Entregar", "ENTREGUE" },
            { "Devolver", "DEVOLVIDO" }
        };

    }
}
