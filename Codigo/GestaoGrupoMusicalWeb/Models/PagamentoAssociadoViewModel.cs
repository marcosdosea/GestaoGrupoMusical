using Core.DTO;

namespace GestaoGrupoMusicalWeb.Models
{
    public class PagamentoAssociadoViewModel
    {
        public FinanceiroCreateViewModel? Financeiro { get; set; }
        public List<AssociadoPagamentoDTO> Associados { get; set; } = new();
        public int IdReceita { get; set; }
    }
}