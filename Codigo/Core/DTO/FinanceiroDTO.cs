using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class FinanceiroIndexDataPage
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Início")]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Final")]
        public DateTime DataFim { get; set; }


        [Display(Name = "Pagos")]
        public int Pagos { get; set; }

        [Display(Name = "Isentos")]
        public int Isentos { get; set; }

        [Display(Name = "Atrasos")]
        public int Atrasos { get; set; }

        [Display(Name = "Recebido")]
        public decimal Recebido { get; set; }

    }
}