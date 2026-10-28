using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class StatusPagamento
    {
        public const string NAO_PAGOU = "NAO_PAGOU";
        public const string PAGO = "PAGO";
        public const string PAGO_COMPROVANTE = "PAGO_COMPROVANTE";
        public const string ISENTO = "ISENTO";

        public static bool EstaPago(string? status) => status == PAGO || status == PAGO_COMPROVANTE;

        public static bool EstaIsento(string? status) => status == ISENTO;

        // Isento não é pendente: só é pendente quem não pagou e não é isento
        public static bool EstaPendente(string? status) => !EstaPago(status) && !EstaIsento(status);
    }
}
