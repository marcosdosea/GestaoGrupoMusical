using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.DTO
{
    public class MovimentacaoFigurinoDTO
    {
        public int Id { get; set; }

        public int IdFigurino { get; set; }
        public int IdManequim { get; set; }

        [Display(Name = "CPF")]
        public string Cpf { get; set; } = string.Empty;

        [Display(Name = "Associado")]
        public string NomeAssociado { get; set; } = string.Empty;

        public DateTime Data { get; set; }

        public string Tamanho { get; set; }

        [Display(Name = "Movimentação")]
        public string Movimentacao { get; set; } = string.Empty;
        [Display(Name ="Quantidade")]
        public int QuantidadeEntregue { get; set; } 
        public string Status { get; set; } = string.Empty;
    }

    public class MovimentarConfirmacaoQuantidade
    {
        public int Quantidade { get; set; }
        public sbyte Confirmar { get; set; }
        public int Id { get; set; }
    }
}
