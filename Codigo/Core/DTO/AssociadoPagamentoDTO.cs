using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AssociadoPagamentoDTO
    {
        public int IdAssociado { get; set; }
        public string NomeAssociado { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal ValorPago { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = "NAO_PAGOU"; // Valor Padrão
    }
}