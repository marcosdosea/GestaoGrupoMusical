
namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        Task<bool> Create(Movimentacaoinstrumento movimentacao);

        Task<Movimentacaoinstrumento?> GetMovimentacaoByIdInstrumento(int idInstrumento);
    }
}
