
namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        Task<bool> Create(Movimentacaoinstrumento movimentacao);
    }
}
