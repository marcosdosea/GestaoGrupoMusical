using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    //aqui eh ReceitaFinanceira
    public class FinanceiroIndexViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao{ get; set; } = string.Empty;

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

        [Display(Name = "Recebidos")]
        public int Recebido { get; set; }

    }
}
