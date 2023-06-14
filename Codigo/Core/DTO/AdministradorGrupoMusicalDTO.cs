using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AdministradorGrupoMusicalDTO
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Display(Name = "CPF")]
        public string? Cpf { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

    }
}
