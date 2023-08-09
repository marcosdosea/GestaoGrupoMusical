using Core.DTO;
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
        public IEnumerable<EstoqueDTO>? TabelaEstoques { get; set; }
    }
}
