using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EstoqueDTOViewModel
    {
        [Display(Prompt = "Nome do figurino")]
        public string? Nome { get; set; }
        public DateTime? Data { get; set; }
        [Display(Name = "Tamanho")]
        public string? Tamanho { get; set; }
        public int Disponivel { get; set; }
        public int Entregues { get; set; }
        public IEnumerable<EstoqueDTOViewModel>? TabelaEstoques { get; set; }
    }
}
