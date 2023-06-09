﻿using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class IdentityViewModel
    {
        public class AutenticarViewModel
        {
            [Display(Name = "CPF")]
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido")]
            public string Cpf { get; set; } = string.Empty;

            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [DataType(DataType.Password)]
            public string Senha { get; set; } = string.Empty;
        }

        public class CadastrarViewModel
        {
            public PessoaViewModel Pessoa { get; set; } = new();

            [Required (ErrorMessage = "O campo {0} é obrigatório")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "A {0} deve contar no mínimo {1} caracteres")]
            public string Senha { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [Compare("Senha", ErrorMessage = "As senhas devem ser iguais")]
            public string ConfirmarSenha { get; set; } = string.Empty;

            public Dictionary<string, string> SexoPessoa { get; } = new()
            {
                { "Feminino", "F" },
                { "Masculino", "M" }
            };
        }
    }
}
