using Core.DTO;

namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        /// <summary>
        /// Cadastra uma movimentação no banco de dados
        /// </summary>
        /// <param name="movimentacao"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Instrumento com status danificado <para />
        /// 401 - Ação de emprestimo/devolução para instrumento já emprestado/devolvido <para />
        /// 402 - Ação de devolução inválida pois associado não corresponde ao mesmo do empréstimo <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> CreateAsync(Movimentacaoinstrumento movimentacao);

        Task<Movimentacaoinstrumento?> GetEmprestimoByIdInstrumento(int idInstrumento);

        Task<IEnumerable<MovimentacaoInstrumentoDTO>> GetAllByIdInstrumento(int idInstrumento);

        Task<bool> Delete(int id);

        Task<bool> NotificarViaEmail(int id);
    }
}
