
using Core.DTO;

namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        Task<bool> Create(Movimentacaoinstrumento movimentacao);

        Task<Movimentacaoinstrumento?> GetEmprestimoByIdInstrumento(int idInstrumento);

        Task<IEnumerable<MovimentacaoInstrumentoDTO>> GetAll();
    }
}
