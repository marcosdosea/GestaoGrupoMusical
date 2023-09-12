﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Models
{
    public class UserViewModel
    {
        [Display(Name = "Nome", Prompt = "Meu Nome")]
        [Required(ErrorMessage = "O campo Nome é obrigatótio.")]
        [StringLength(70, MinimumLength = 5, ErrorMessage = "O nome do associado deve ter entre 5 e 70 caracteres")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo Sexo é obrigatório.")]
        [StringLength(1)]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O CEP deve estar no formato 00000-000.")]
        [StringLength(10)]
        [Display(Name = "CEP", Prompt = "00000-000")]
        public string? Cep { get; set; }

        [StringLength(70)]
        [Display(Name = "Rua", Prompt = "Rua")]
        public string? Rua { get; set; }

        [StringLength(70)]
        [Display(Name = "Bairro", Prompt = "Rua")]
        public string? Bairro { get; set; }

        [StringLength(70)]
        [Display(Name = "Cidade", Prompt = "Cidade")]
        public string? Cidade { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [StringLength(2)]
        [Display(Name = "Estado", Prompt = "Estado")]
        public string? Estado { get; set; }

        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date, ErrorMessage = "É necessário escolher uma data válida.")]
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage = "O campo telefone é obrigatório.")]
        [StringLength(20)]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        [Display(Name = "Telefone 1", Prompt = "(99)99999-9999")]
        public string? Telefone1 { get; set; }

        [Display(Name = "Telefone 2", Prompt = "(99)99999-9999")]
        [StringLength(20)]
        [RegularExpression(@"^\(\d{2}\)\d{5}-\d{4}$", ErrorMessage = "O telefone deve estar no formato (XX) XXXXX-XXXX.")]
        public string? Telefone2 { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "O email está no formato inválido.")]
        [Display(Name = "E-mail", Prompt = "exemplo@gmail.com")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "O campo Tamanho da roupa é obrigatório.")]
        [Display(Name = "Tamanho da roupa")]
        public int IdManequim { get; set; }
        public SelectList? ListaManequim { get; set; }
        public Dictionary<string, char> sexoPessoa { get; } = new()
        {
            { "Masculino", 'M' },
            { "Feminino", 'F' }
        };

        [Display(Name = "Nova Senha")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Confirmar Senha")]
        [RegularExpression(@"^.{6,}$", ErrorMessage = "Mínimo de 6 caracteres")]
        [Compare("Password", ErrorMessage = "Senhas não são iguais")]
        [DataType(DataType.Password)]
        public string? ConfirmePassword { get; set; }
    }

}