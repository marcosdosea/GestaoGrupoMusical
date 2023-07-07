﻿using Core.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class AdministradorGrupoMusicalViewModel
    {
        public class AdministradorModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "O campo Nome é obrigatótio.")]
            [StringLength(70, MinimumLength = 5, ErrorMessage = "O nome do associado deve ter entre 5 e 70 caracteres")]
            public string? Nome { get; set; }

            [Required(ErrorMessage = "O campo de CPF é obrigatório.")]
            [StringLength(15)]
            public string? Cpf { get; set; }

            [Required(ErrorMessage = "O e-mail é obrigatório!")]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
            [StringLength(1)]
            public string? Sexo { get; set; }

            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [Display(Name = "Grupo Musical")]
            public string? IdGrupoMusical { get; set; }

            public SelectList? ListaGrupoMusical { get; set; }
        }

        public IEnumerable<AdministradorGrupoMusicalDTO>? ListaAdministrador { get; set; }

    }
}
