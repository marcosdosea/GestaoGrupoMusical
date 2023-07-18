﻿using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

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

        public class ForgotPasswordViewModel
        {
            [Display(Name = "Email")]
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            public string Email { get; set; }
        }

        public class ResetPasswordViewModel
        {
            public int UserId { get; set; }
            public string Code { get; set; }

            [Display(Name = "Nova Senha")]
            [Required(ErrorMessage = "Campo obrigatório")]
            [RegularExpression(@"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Pc}\p{Pd}\p{Ps}\p{Pe}\p{Pi}\p{Pf}\p{Po}]", ErrorMessage = "Senha deve conter no mínimo 8 caracteres,\nno máximo 16 e ao menos 1 caracter especial")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Confirmar Senha")]
            [Required(ErrorMessage = "Campo obrigatório")]
            [RegularExpression(@"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Pc}\p{Pd}\p{Ps}\p{Pe}\p{Pi}\p{Pf}\p{Po}]", ErrorMessage = "Senha deve conter no mínimo 8 caracteres,\nno máximo 16 e ao menos 1 caracter especial")]
            public string ConfirmePassword { get; set; } = string.Empty;
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
