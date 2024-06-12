using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class MaterialEstudoViewModel
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Título")]
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O título do material deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Link")]
        [Required(ErrorMessage = "O link do material é obrigatório")]
        [StringLength(500, ErrorMessage = "O link do material deve ter até 500 caracteres")]
        public string Link { get; set; } = null!;

        public DateTime Data { get; set; }

        [Display(Name = "Grupo Musical")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdGrupoMusical { get; set; }
        [Display(Name = "Colaborador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int IdColaborador { get; set; }
    }
}
