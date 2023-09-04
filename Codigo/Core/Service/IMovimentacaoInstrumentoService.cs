using Core.DTO;
using System.Net;
using static Core.DTO.InstrumentoAssociadoDTO;

namespace Core.Service
{
    public interface IMovimentacaoInstrumentoService
    {
        /// <summary>
        /// Cadastra uma movimentação no banco de dados
        /// </summary>
        /// <param name="movimentacao"></param>
        /// <returns>
        /// Created             - Sucesso <para />
        /// BadRequest          - Instrumento com status danificado <para />
        /// Conflict            - Ação de emprestimo/devolução para instrumento já emprestado/devolvido <para />
        /// PreconditionFailed  - Ação de devolução inválida pois associado não corresponde ao mesmo do empréstimo <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> CreateAsync(Movimentacaoinstrumento movimentacao);

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
        Task<HttpStatusCode> DeleteAsync(int id);

        /// <summary>
        /// Envia uma notificação sobre o empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 401 - Instrumento não está cadastrado no sistema <para />
        /// 402 - Associado não está cadastrado no sistema <para />
        /// 404 - O id não corresponde a nenhuma movimentação <para />
        /// 406 - Empréstimo já está confirmado <para />
        /// 407 - Devolução já está confirmada <para />
        /// 500 - Erro interno
        /// </returns>
        Task<HttpStatusCode> NotificarViaEmailAsync(int id);

        Task<MovimentacoesAssociado> MovimentacoesByIdAssociadoAsync(int idAssociado);

        /// <summary>
        /// Confirmar um empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="idMovimentacao"></param>
        /// <param name="idAssociado"></param>
        /// <returns>
        /// 200 - Sucesso Empréstimo <para />
        /// 201 - Sucesso Devolução <para />
        /// 400 - Associado inválido para empréstimo <para />
        /// 401 - Associado inválido para devolução <para />
        /// 404 - O id não corresponde a nenhuma movimentação <para />
        /// 500 - Erro interno
        /// </returns>
        Task<HttpStatusCode> ConfirmarMovimentacaoAsync(int idMovimentacao, int idAssociado);
    }
}
