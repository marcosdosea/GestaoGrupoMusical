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
        /// Remove uma movimentação no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> DeleteAsync(int id);

        Task<IEnumerable<EstoqueDTO>> GetEstoque(int idFigurino);

        Task<MovimentacoesAssociadoFigurino> MovimentacoesByIdAssociadoAsync(int idAssociado);
    }
}
