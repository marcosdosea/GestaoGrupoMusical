using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 200 - Sucesso
        /// 500 - Erro interno
        /// </returns>
        Task<int> CreateAsync(Movimentacaofigurino movimentacao);

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
        Task<bool> AssociadoEmprestimo(int idAssociado, int idFigurino,int idManequim);

        /// <summary>
        /// remove uma movimentação de figurino
        /// </summary>
        /// <param name="id">id da movimentação</param>
        /// <returns>
        /// 200: tudo ocorreu bem
        /// 400: movimentacao nao foi encontrada
        /// 500: algo deu errado ao remover/salvar a transação
        /// </returns>
        Task<int> DeleteAsync(int id);

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
        /// 200 - Sucesso Empréstimo <para />
        /// 201 - Sucesso Devolução <para />
        /// 400 - Associado inválido para empréstimo <para />
        /// 401 - Associado inválido para devolução <para />
        /// 404 - O id não corresponde a nenhuma movimentação <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> ConfirmarMovimentacao(int idMovimentacao, int idAssociado);
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
