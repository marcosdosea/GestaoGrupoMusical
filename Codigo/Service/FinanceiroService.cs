using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;

namespace Service
{
    public class FinanceiroService : IFinanceiroService
    {
        private readonly GrupoMusicalContext _context;

        public FinanceiroService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public FinanceiroStatus Create (FinanceiroCreateDTO rf)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Add(rf);
                _context.SaveChanges();

                return FinanceiroStatus.Success;
            }
            catch
            {
                return FinanceiroStatus.Error;
            }

           
        }

        public IEnumerable<FinanceiroIndexDataPage> GetAllFinanceiroPorIdGrupo(int idGrupoMusical)
        {
            DateTime dataMesesAtrasados = DateTime.Now.Date;
            var query = (from financeiro in _context.Receitafinanceiras
                         where financeiro.IdGrupoMusical == idGrupoMusical
                         select new FinanceiroIndexDataPage
                         {
                             Id = financeiro.Id,
                             Descricao = financeiro.Descricao,
                             DataInicio = financeiro.DataInicio,
                             DataFim = financeiro.DataFim,
                             Pagos = financeiro.Receitafinanceirapessoas.
                             Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                              && rfp.Status == TipoPagamento.PAGO.ToString()).Count(),
                             Isentos = financeiro.Receitafinanceirapessoas.
                             Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                              && rfp.Status == TipoPagamento.ISENTO.ToString()).Count(),
                             Atrasos = financeiro.Receitafinanceirapessoas.
                             Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                              && rfp.Status == TipoPagamento.ABERTO.ToString()
                              && financeiro.DataFim < dataMesesAtrasados).Count(),
                             Recebido = financeiro.Receitafinanceirapessoas.Where(rfp => rfp.Status == "PAGO").
                             Sum(rfp => rfp.ValorPago),
                         }).ToList();
            if (query.Count() > 0)
            {
                foreach (var item in query)
                {
                    if (item.Descricao.Length > 15)
                    {
                        item.Descricao = item.Descricao.Substring(0, 15) + "...";
                    }
                }
            }
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
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    financeiroIndexDTO = financeiroIndexDTO.OrderByDescending(g => g.Descricao);
                else
                    financeiroIndexDTO = financeiroIndexDTO.OrderBy(g => g.Descricao);
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
            financeiroIndexDTO = financeiroIndexDTO.Skip(request.Start).Take(request.Length);
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