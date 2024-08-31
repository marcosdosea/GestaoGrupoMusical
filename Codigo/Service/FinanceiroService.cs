using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FinanceiroService : IFinanceiroService
    {
        private readonly GrupoMusicalContext _context;
        public FinanceiroService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinanceiroIndexDataPage>> GetAllFinanceiroPorIdGrupo(int idGrupoMusical, int mesesAtrasados)
        {
            DateTime dataMesesAtrasados = DateTime.Now.Date;
            var query = await (from financeiro in _context.Receitafinanceiras
                               where financeiro.IdGrupoMusical == idGrupoMusical
                               select new FinanceiroIndexDataPage
                               {
                                   Id = financeiro.Id,
                                   DataInicio = financeiro.DataInicio,
                                   DataFim = financeiro.DataFim,
                                   Pagos = financeiro.Receitafinanceirapessoas.
                                   Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                                    && rfp.Status == "PAGO").Count(),
                                   Isentos = financeiro.Receitafinanceirapessoas.
                                   Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                                    && rfp.Status == "ISENTO").Count(),
                                   Atrasos = financeiro.Receitafinanceirapessoas.
                                   Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                                    && rfp.Status == "ENVIADO"
                                    && financeiro.DataFim < dataMesesAtrasados).Count(),
                                   Recebido = financeiro.Receitafinanceirapessoas.
                                   Sum(rfp => rfp.ValorPago),
                               }).ToListAsync();
            return query;
        }


        public DatatableResponse<FinanceiroIndexDataPage> GetDataPage(DatatableRequest request, IEnumerable<FinanceiroIndexDataPage> financeiroIndexDTO)
        {
            var totalRecords = financeiroIndexDTO.Count();
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                financeiroIndexDTO = financeiroIndexDTO.Where
                    (g => g.Descricao.ToString().Contains(request.Search.GetValueOrDefault("value")!));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("0"))
            {
                if (!request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                {
                    financeiroIndexDTO = financeiroIndexDTO.OrderByDescending(g => g.Descricao);
                }
                else
                {
                    financeiroIndexDTO = financeiroIndexDTO.OrderBy(g => g.Descricao);
                }
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    financeiroIndexDTO = financeiroIndexDTO.OrderBy(g => g.DataInicio);
                else
                    financeiroIndexDTO = financeiroIndexDTO.OrderByDescending(g => g.DataInicio);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    financeiroIndexDTO = financeiroIndexDTO.OrderBy(g => g.DataFim);
                else
                    financeiroIndexDTO = financeiroIndexDTO.OrderByDescending(g => g.DataFim);
            }

            int countRecordsFiltered = financeiroIndexDTO.Count();
            //financeiroIndexDTO = financeiroIndexDTO.Skip(request.Start).Take(request.Length);
            return new DatatableResponse<FinanceiroIndexDataPage>
            {
                Data = financeiroIndexDTO.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }
    }
}
