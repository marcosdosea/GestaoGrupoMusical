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

        /// <summary>
        /// Remove uma movimentação no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Movimentação de emprestimo com instrumento não devolvido <para />
        /// 404 - O id não corresponde a nenhuma movimentação <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> DeleteAsync(int id);

        /// <summary>
        /// Envia uma notificação sobre o empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 401 - Instrumento não está cadastrado no sistema <para />
        /// 402 - Associado não está cadastrado no sistema <para />
        /// 404 - O id não corresponde a nenhuma movimentação <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> NotificarViaEmailAsync(int id);
    }
}
