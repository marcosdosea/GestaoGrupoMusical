using Core.DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EstoqueViewModel
    {
        public int Id { get; set; }
        [Display(Prompt = "Nome do figurino")]
        public string? Nome { get; set; }
        public DateTime? Data { get; set; }
        public IEnumerable<EstoqueDTO>? TabelaEstoques { get; set; }
    }

    public class CreateEstoqueViewModel
    {
        [Required]
        public string? Nome { get; set; }

        [Required]
        public string Data { get; set; }

        [Display(Name = "Tamanho")]
        [Required(ErrorMessage = "O campo Tamanho é obrigatório.")]
        public int IdManequim { get; set; }

        [Required]
        public int IdFigurino { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int QuantidadeDisponivel { get; set; }

        public SelectList? listManequim { get; set; }
    }
}
