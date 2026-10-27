using Core.DTO;
using Core.Service;

namespace GestaoGrupoMusicalWeb.Models
{
    public class PagamentoAssociadoViewModel
    {
        public FinanceiroCreateViewModel? Financeiro { get; set; }
        public List<AssociadoPagamentoDTO> Associados { get; set; } = new();
        public int IdReceita { get; set; }
        public SituacaoPagamentoFiltro SituacaoFiltro { get; set; } = SituacaoPagamentoFiltro.Todos;
    }
}