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
        /// OK - Sucesso <para />
        /// PreconditionFailed - Movimentação de emprestimo com instrumento não devolvido <para />
        /// NotFound - O id não corresponde a nenhuma movimentação <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> DeleteAsync(int id);

        /// <summary>
        /// Envia uma notificação sobre o empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// ok - Sucesso <para />
        /// 401 - Instrumento não está cadastrado no sistema <para />
        /// 402 - Associado não está cadastrado no sistema <para />
        /// NotFound - O id não corresponde a nenhuma movimentação <para />
        /// 406 - Empréstimo já está confirmado <para />
        /// 407 - Devolução já está confirmada <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> NotificarViaEmailAsync(int id);

        Task<MovimentacoesAssociado> MovimentacoesByIdAssociadoAsync(int idAssociado);

        /// <summary>
        /// Confirmar um empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="idMovimentacao"></param>
        /// <param name="idAssociado"></param>
        /// <returns>
        /// Created - Sucesso Empréstimo <para />
        /// OK - Sucesso Devolução <para />
        /// PreconditionFailed - Associado inválido para empréstimo <para />
        /// BadRequest - Associado inválido para devolução <para />
        /// NotFound - O id não corresponde a nenhuma movimentação <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> ConfirmarMovimentacaoAsync(int idMovimentacao, int idAssociado);
    }
}
