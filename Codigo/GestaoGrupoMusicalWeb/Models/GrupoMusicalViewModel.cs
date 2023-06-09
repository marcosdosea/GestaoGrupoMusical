﻿using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class GrupoMusicalViewModel
    {
        [Key]
        [Display(Name = "Codigo")]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O Campo Nome deve ter entre 5 a 100 caracteres")]
        public string Nome { get; set; } = null!;

        [Required]
        [Display(Name = " Razão Social")]
        [StringLength(100, MinimumLength = 5, ErrorMessage ="O campo Razão Social deve ter entre 5 a 100 caracteres")]
        public string RazaoSocial { get; set; } = null!;

        [Required]
        [Display(Name = "CNPJ")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$", ErrorMessage = "CNPJ inválido")]
        [StringLength(18, MinimumLength = 18, ErrorMessage ="O campo CNPJ deve ter 14 caracteres")]
        public string Cnpj { get; set; } = null!;

        [Display(Name = "CEP")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
        [StringLength(9, MinimumLength = 9,ErrorMessage ="O campo CEP deve ter 8 caracteres")]
        public string? Cep { get; set; }

        [Display(Name ="Rua")]
        [StringLength(100, MinimumLength = 5, ErrorMessage ="O campo rua deve ter entre 5 a 100 caracteres")]
        public string? Rua { get; set; }

        [Display(Name ="Bairro")]
        [StringLength(100, MinimumLength = 5, ErrorMessage ="O campo Bairro deve ter entre 5 a 100 caracteres")]
        public string? Bairro { get; set; }

        [Display(Name ="Cidade")]
        [StringLength(100, MinimumLength = 5, ErrorMessage ="O campo Cidade de ter entre 5 a 100 caracteres")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage ="O campo Estado é obrigatório")]
        [Display(Name = "Estado")]
        [StringLength(2, MinimumLength = 2)]
        public string Estado { get; set; } = null!;

        [Display(Name ="Pais")]
        [StringLength(50,MinimumLength = 4, ErrorMessage =" O campo Pais deve ter de 4 a 50 caracteres")]
        public string? Pais { get; set; }

        [EmailAddress(ErrorMessage = "Endereço de Email Inválido")]
        public string? Email { get; set; }

        [Display(Name ="YouTube")]
        public string? Youtube { get; set; }

        [Display(Name ="Instagram")]
        public string? Instagram { get; set; }

        [Display(Name = "Facebook")]
        public string? Facebook { get; set; }

        [Display(Name = "Telefone 1")]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [StringLength(20)]
        public string? Telefone1 { get; set; }

        [Display(Name = " Telefone 2")]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [StringLength (20)]
        public string? Telefone2 { get; set; }

        [Display(Name = "Banco")]
        public string? Banco { get; set; }

        [Display(Name = "Agência")]
        [StringLength(15)]
        public string? Agencia { get; set; }

        [Display(Name = "Número da Conta do Banco")]
        [StringLength(15)]
        public string? NumeroContaBanco { get; set; }

        [Display(Name = "Chave Pix")]
        public string? ChavePix { get; set; }

        [Display(Name =" Tipo da cave Pix")]
        [StringLength(15)]
        public string? ChavePixtipo { get; set; }
    }
}
