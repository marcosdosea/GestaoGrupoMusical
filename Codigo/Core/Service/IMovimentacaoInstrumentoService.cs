using Core.DTO;

namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        Task<bool> Create(Movimentacaoinstrumento movimentacao);

        Task<Movimentacaoinstrumento?> GetEmprestimoByIdInstrumento(int idInstrumento);

        Task<IEnumerable<MovimentacaoInstrumentoDTO>> GetAllByIdInstrumento(int idInstrumento);

        Task<bool> Delete(int id);

        Task<bool> NotificarViaEmail(int id);
    }
}
