using System.ComponentModel.DataAnnotations;

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

    public class FinanceiroCreateDTO
    {
        [Key]
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public decimal? Valor { get; set; }
        public int IdGrupoMusical { get; set; }
        public IEnumerable<int>? IdAssociados { get; set; }
    }
}