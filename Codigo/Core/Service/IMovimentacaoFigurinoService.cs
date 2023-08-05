using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Task<Movimentacaofigurino?> GetEmprestimoByIdInstrumento(int idFigurino);

        Task<IEnumerable<MovimentacaoFigurinoDTO>> GetAllByIdInstrumento(int idFigurino);

        /// <summary>
        /// Remove uma movimentação no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> DeleteAsync(int id);
    }
}
