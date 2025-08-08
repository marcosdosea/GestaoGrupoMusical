using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore; 
using System.Net;

namespace Service
{
    public class FinanceiroService : IFinanceiroService
    {
        private readonly GrupoMusicalContext _context;

        public FinanceiroService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public FinanceiroStatus Create(FinanceiroCreateDTO rf)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (rf.DataInicio > rf.DataFim)
                {
                    return FinanceiroStatus.DataInicioMaiorQueDataFim;
                }
                if (rf.DataFim < DateTime.Now.Date) 
                {
                    return FinanceiroStatus.DataFimMenorQueDataDeHoje;
                }
                if (rf.Valor <= 0)
                {
                    return FinanceiroStatus.ValorZeroOuNegativo;
                }

                Receitafinanceira f = new Receitafinanceira
                {
                    DataInicio = rf.DataInicio ?? DateTime.Now,
                    DataFim = rf.DataFim ?? DateTime.Now,
                    Valor = rf.Valor ?? 0,
                    Descricao = rf.Descricao,
                    IdGrupoMusical = rf.IdGrupoMusical
                };
                _context.Add(f);
                _context.SaveChanges();
                rf.IdAssociados = _context.Pessoas.Where(p => p.IdGrupoMusical == rf.IdGrupoMusical && p.IdPapelGrupo == 1).Select(p => p.Id);

                if (rf.IdAssociados != null)
                {
                    List<Receitafinanceirapessoa> p = new List<Receitafinanceirapessoa>();
                    foreach (int idAssociado in rf.IdAssociados)
                    {
                        p.Add(new Receitafinanceirapessoa()
                        {
                            IdReceitaFinanceira = f.Id,
                            IdPessoa = idAssociado,
                            Valor = rf.Valor ?? 0,
                            DataPagamento = DateTime.Now,
                        });
                    }
                    _context.AddRange(p);
                    _context.SaveChanges();
                }
                transaction.Commit();
                return FinanceiroStatus.Success;
            }
            catch
            {
                transaction.Rollback(); 
                return FinanceiroStatus.Error;
            }
        }

        // MÉTODO EDIT ADICIONADO AQUI
        public FinanceiroStatus Edit(Receitafinanceira financeiro)
        {
            if (financeiro.DataInicio > financeiro.DataFim)
            {
                return FinanceiroStatus.DataInicioMaiorQueDataFim;
            }
            if (financeiro.DataFim < DateTime.Now.Date) 
            {
                return FinanceiroStatus.DataFimMenorQueDataDeHoje;
            }
            if (financeiro.Valor <= 0)
            {
                return FinanceiroStatus.ValorZeroOuNegativo;
            }

            try
            {
                _context.Update(financeiro);
                _context.SaveChanges();
                return FinanceiroStatus.Success;
            }
            catch
            {
                return FinanceiroStatus.Error;
            }
        }

        // MÉTODO DELETE ADICIONADO AQUI
        public void Delete(int id)
        {
            var financeiro = _context.Receitafinanceiras.Find(id);
            if (financeiro != null)
            {
                var pagamentosAssociados = _context.Receitafinanceirapessoas.Where(p => p.IdReceitaFinanceira == id);
                _context.Receitafinanceirapessoas.RemoveRange(pagamentosAssociados);

                _context.Receitafinanceiras.Remove(financeiro);
                _context.SaveChanges();
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
                    (g => g.Descricao.ToString().ToLower().Contains(request.Search.GetValueOrDefault("value")!.ToLower()));
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

        public Receitafinanceira? Get(int id)
        {
            return _context.Receitafinanceiras.Find(id);
        }

        public HttpStatusCode NotificarFinanceiroViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idFinanceiro)
        {
            try
            {
                var financeiro = Get(idFinanceiro);
                if (financeiro != null)
                {
                    List<EmailModel> emailsBody = new List<EmailModel>();
                    foreach (PessoaEnviarEmailDTO p in pessoas)
                    {
                        emailsBody.Add(new EmailModel()
                        {
                            Assunto = $"Batalá - Notificação de Pagamento: {financeiro.Valor} foram pagos",
                            AddresseeName = p.Nome,
                            Body = "<div style=\"text-align: center;\">\r\n   " +
                                   $"<h3>O pagamento foi aprovado!</h3>\r\n</div>",
                            To = new List<string> { p.Email }

                        });
                    }

                    List<Task> emailTask = new List<Task>();
                    foreach (EmailModel email in emailsBody)
                    {
                        emailTask.Add(EmailService.Enviar(email));
                    }

                    return HttpStatusCode.OK;
                }

                return HttpStatusCode.NotFound;
            }

            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}