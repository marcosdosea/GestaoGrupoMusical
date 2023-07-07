using Core.DTO;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class AdministradorGrupoMusicalViewModel
    {
        public class AdministradorModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "O campo Nome é obrigatótio.")]
            [StringLength(70, MinimumLength = 5, ErrorMessage = "O nome deve ter entre 5 e 70 caracteres")]
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo {0} é obrigatório.")]
            [Display(Name = "CPF")]
            [StringLength(15)]
            public string Cpf { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo {0} é obrigatório!")]
            [Display(Name = "E-mail")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
            [StringLength(1)]
            public string Sexo { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [Display(Name = "Grupo Musical")]
            public string NomeGrupoMusical { get; set; } = string.Empty;

            public int IdGrupoMusical { get; set; }
        }

        public AdministradorModel Administrador { get; set; } = new();

        public IEnumerable<AdministradorGrupoMusicalDTO>? ListaAdministrador { get; set; }

    }
}
