using Core.Datatables;
using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IFinanceiroService
    {
        FinanceiroStatus Create(FinanceiroCreateDTO rf);
        IEnumerable<FinanceiroIndexDataPage> GetAllFinanceiroPorIdGrupo(int idGrupoMusical);
        DatatableResponse<FinanceiroIndexDataPage> GetDataPage(DatatableRequest request, IEnumerable<FinanceiroIndexDataPage> materialEstudoIndexDTO);
        HttpStatusCode NotificarFinanceiroViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idFinanceiro);
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
