using Core;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class FinanceiroCreateViewModel
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Descrição obrigatória")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A descrição precisa ter entre 3 a 100 caracteres")]
        public string Descricao { get; set; } = string.Empty;


        [Display(Name = "Data Início")]
        [Required(ErrorMessage = "A data início é obrigatória")]
        public DateTime? DataInicio { get; set; }

        [Display(Name = "Data Final")]
        [Required(ErrorMessage = "A data final é obrigatória")]
        public DateTime? DataFim { get; set; }


        [Display(Name = "Valor (R$)")]
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, 99999999.99, ErrorMessage = "O valor deve ser maior que zero e ter até duas casas decimais.")]
        [RegularExpression(@"^\d{1,8}(\.\d{1,2})?$", ErrorMessage = "O valor deve ter até 8 dígitos inteiros e 2 casas decimais.")]

        public decimal? Valor { get; set; }
        public int IdGrupoMusical { get; set; }
    }
}
