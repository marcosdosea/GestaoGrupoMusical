using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class FinanceiroIndexDTO
    {
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }


        public int Pagos { get; set; }

        public int Isentos { get; set; }

        public int Atrasos { get; set; }

        public int Recebido { get; set; }

    }
}
