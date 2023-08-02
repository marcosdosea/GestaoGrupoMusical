using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GestaoGrupoMusicalWeb.Models
{
    public class FigurinoViewModel
    {
        [Required(ErrorMessage = "O código do associado é obrigatótio.")]
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Nome", Prompt = "Ex.: São João 2023")]
        [Required(ErrorMessage = "O campo Nome é obrigatótio.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O nome do figurino deve ter entre 5 e 100 caracteres")]
        public string? Name { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date, ErrorMessage = "É necessário escolher uma data válida.")
        [Required(ErrorMessage = "O campo Data é obrigatótio.")]
        public DateTime? dateTime { get; set; }

    }
}
