using Core.Datatables;
using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IFinanceiroService
    {
        FinanceiroStatus Create(FinanceiroCreateDTO rf);
        FinanceiroStatus Edit(Receitafinanceira financeiro);
        void Delete(int id);

        // ESTA LINHA ABAIXO É A QUE ESTÁ FALTANDO NO SEU CÓDIGO
        Receitafinanceira? Get(int id);

        IEnumerable<FinanceiroIndexDataPage> GetAllFinanceiroPorIdGrupo(int idGrupoMusical);
        DatatableResponse<FinanceiroIndexDataPage> GetDataPage(DatatableRequest request, IEnumerable<FinanceiroIndexDataPage> materialEstudoIndexDTO);
        HttpStatusCode NotificarFinanceiroViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idFinanceiro);

        Task<IEnumerable<AssociadoPagamentoDTO>> GetAssociadosPagamento(int idReceita);
        Task SalvarPagamentos(int idReceita, IEnumerable<AssociadoPagamentoDTO> associados);
    }


    public enum FinanceiroStatus
    {
        Error,
        Success,
        DataInicioMaiorQueDataFim,
        DataFimMenorQueDataDeHoje,
        ValorZeroOuNegativo
    }

    public enum TipoPagamento
    {
        ABERTO,
        PAGO,
        ISENTO,
    }
}