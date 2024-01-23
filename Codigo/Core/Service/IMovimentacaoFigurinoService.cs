using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Core.DTO.InstrumentoAssociadoDTO;
using static Core.DTO.MovimentacaoAssociadoFigurinoDTO;

namespace Core.Service
{
    public interface IMovimentacaoFigurinoService
    {
        /// <summary>
        /// Cadastra uma movimentação no banco de dados
        /// </summary>
        /// <param name="movimentacao"></param>
        /// <returns>
        /// OK - Sucesso
        /// NoContent; //não há peças disponiveis para emprestar
        /// NotFound; //estoque nao existe, talvez id esteja errado
        /// PreconditionFailed; //associado nao possue nada emprestado para devolver
        /// FailedDependency; //não houve confirmação
        /// BadRequest; //tentativa de devolução de figurino a mais ou a menos da quantidade que o associado possui
        /// 
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> CreateAsync(Movimentacaofigurino movimentacao);

        Task<Movimentacaofigurino?> GetEmprestimoByIdFigurino(int idFigurino);

        Task<IEnumerable<MovimentacaoFigurinoDTO>> GetAllByIdFigurino(int idFigurino);

        /// <summary>
        /// Este metodo retorna se o associado possui alguma
        /// emprestimo com o figurino do tamanho "manequim"
        /// </summary>
        /// <param name="idAssociado">id do associado</param>
        /// <param name="idFigurino">id do figurino</param>
        /// <param name="idManequim">id do manequim(tamanho)</param>
        /// <returns>true: nao possue emprestimo `ativo`, false: possue emprestimo `ativo`</returns>
        Task<bool> AssociadoEmprestimo(int idAssociado, int idFigurino, int idManequim);

        /// <summary>
        /// remove uma movimentação de figurino
        /// </summary>
        /// <param name="id">id da movimentação</param>
        /// <returns>
        /// OK: tudo ocorreu bem
        /// NotFound: movimentacao nao foi encontrada
        /// InternalServerError: algo deu errado ao remover/salvar a transação
        /// </returns>
        Task<HttpStatusCode> DeleteAsync(int id);

        /// <summary>
        /// Busca uma movimentação de figurino
        /// </summary>
        /// <param name="id">código da movimentação</param>
        /// <returns>movimentação do figurino</returns>
        Task<Movimentacaofigurino?> Get(int id);

        Task<IEnumerable<EstoqueDTO>> GetEstoque(int idFigurino);
        
        /// <summary>
        /// Consulta os emprestimo e devolucao do usuario
        /// </summary>
        /// <param name="MovimentacoesByIdAssociadoAsync"></param>
        /// <returns>
        /// retorna os Enumerable de emprestimo e devolução do associado
        /// </returns>
        Task<MovimentacoesAssociadoFigurino> MovimentacoesByIdAssociadoAsync(int idAssociado);

        /// <summary>
        /// Confirmar um empréstimo/devolução de instrumento
        /// </summary>
        /// <param name="idMovimentacao"></param>
        /// <param name="idAssociado"></param>
        /// <returns>
        /// Created - Sucesso Empréstimo <para />
        /// ok - Sucesso Devolução <para />
        /// PreconditionFailed - Associado inválido para empréstimo <para />
        /// FailedDependency - Associado inválido para devolução <para />
        /// NotFound - O id não corresponde a nenhuma movimentação <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> ConfirmarMovimentacao(int idMovimentacao, int idAssociado);

        /// <summary>
        /// Buscar a confirmação do usuario
        /// </summary>
        /// <param name="idAssociado"></param>
        /// <param name="idFigurino"></param>
        /// <param name="idManequim"></param>
        /// <returns>
        /// Um objeto de movimentarComfirmacaooQuandtidade 
        /// </returns>
        Task<MovimentarConfirmacaoQuantidade> GetConfirmacaoFigurino(int idAssociado, int idFigurino, int idManequim);
    }
}
