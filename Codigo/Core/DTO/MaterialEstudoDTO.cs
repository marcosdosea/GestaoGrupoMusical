using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class MaterialEstudoIndexDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Material")]
        [Required(ErrorMessage = "O nome do Material é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome do material deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Link")]
        [Required(ErrorMessage = "O link do material é obrigatório")]
        [StringLength(500, ErrorMessage = "O link do material deve ter até 500 caracteres")]
        public string Link { get; set; } = null!;

        public DateTime? Data { get; set; }
    }
}
