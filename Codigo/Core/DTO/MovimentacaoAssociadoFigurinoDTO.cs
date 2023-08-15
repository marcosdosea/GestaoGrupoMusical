using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DTO.InstrumentoAssociadoDTO;
using System.Xml.Linq;

namespace Core.DTO
{
     public class MovimentacaoAssociadoFigurinoDTO
    {
        public class MovimentacaoAssociadoFigurino
        {
            public int Id { get; set; }
            [Display(Name ="Data")]
            public DateTime Data { get; set; }
            [Display(Name ="Nome Figurino")]
            public string NomeFigurino { get; set; }
            [Display(Name ="Tamanho")]
            public string Tamanho { get; set; }
            [Display(Name="Status")]
            public string Status { get; set; }
        }

        public class MovimentacoesAssociadoFigurino
        {
            [Display(Name = "Entregues")]
            public IEnumerable<MovimentacaoAssociadoFigurino>? Entregue { get; set; }

            [Display(Name = "Devoluções")]
            public IEnumerable<MovimentacaoAssociadoFigurino>? Devolucoes { get; set; }
        }
    }
}
