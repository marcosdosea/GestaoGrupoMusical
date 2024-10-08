﻿using Core.Datatables;
using Core.DTO;

namespace Core.Service
{
    public interface IFinanceiroService
    {
        FinanceiroStatus Create(FinanceiroCreateDTO rf);
        IEnumerable<FinanceiroIndexDataPage> GetAllFinanceiroPorIdGrupo(int idGrupoMusical);
        DatatableResponse<FinanceiroIndexDataPage> GetDataPage(DatatableRequest request, IEnumerable<FinanceiroIndexDataPage> materialEstudoIndexDTO);
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
