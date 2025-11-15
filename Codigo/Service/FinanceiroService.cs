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

                // devolve ID gerado pelo EF
                rf.Id = f.Id;

                // 1. Busca todos os associados ATIVOS, já trazendo a informação de isenção
                var associados = _context.Pessoas
                    .Where(p => p.IdGrupoMusical == rf.IdGrupoMusical && p.IdPapelGrupo == 1 && p.Ativo == 1)
                    .Select(p => new { p.Id, p.IsentoPagamento })
                    .ToList();

                if (associados.Any())
                {
                    var pagamentos = new List<Receitafinanceirapessoa>();
                    foreach (var associado in associados)
                    {
                        pagamentos.Add(new Receitafinanceirapessoa
                        {
                            IdReceitaFinanceira = f.Id,
                            IdPessoa = associado.Id,
                            Valor = rf.Valor ?? 0,
                            // 2. Define o status como ISENTO ou NAO_PAGOU no momento da criação
                            Status = (associado.IsentoPagamento == 1) ? StatusPagamento.ISENTO : StatusPagamento.NAO_PAGOU,
                            
                        });
                    }
                    _context.AddRange(pagamentos);
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
                                     && (rfp.Status == StatusPagamento.PAGO || rfp.Status == StatusPagamento.PAGO_COMPROVANTE)).Count(),
                             Isentos = financeiro.Receitafinanceirapessoas.
                                 Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id
                                     && rfp.Status == TipoPagamento.ISENTO.ToString()).Count(),
                             Atrasos = financeiro.Receitafinanceirapessoas.
                                 Where(rfp => rfp.IdReceitaFinanceira == financeiro.Id &&
                                             ((rfp.Status == StatusPagamento.PAGO || rfp.Status == StatusPagamento.PAGO_COMPROVANTE) && rfp.DataPagamento > financeiro.DataFim) ||
                                             (rfp.Status == StatusPagamento.NAO_PAGOU && financeiro.DataFim < DateTime.Now.Date)
                                 ).Count(),
                          
                             Recebido = financeiro.Receitafinanceirapessoas
                                 .Where(rfp => rfp.Status == StatusPagamento.PAGO || rfp.Status == StatusPagamento.PAGO_COMPROVANTE)
                                 .Sum(rfp => rfp.Valor), 
                                                         
                         }).ToList();

            if (query.Any())
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


        public async Task<IEnumerable<AssociadoPagamentoDTO>> GetAssociadosPagamento(int idReceita)
        {
            // Busca a receita para obter o Id do Grupo Musical
            var receita = await _context.Receitafinanceiras.FindAsync(idReceita);
            if (receita == null)
            {
                // Retorna uma lista vazia se a receita não for encontrada
                return new List<AssociadoPagamentoDTO>();
            }
            var idGrupoMusical = receita.IdGrupoMusical;

            // Busca todos os associados ATIVOS do grupo musical correto,
            // incluindo a informação se ele é ISENTO
            var associadosDoGrupo = await _context.Pessoas
                .Where(p => p.IdGrupoMusical == idGrupoMusical && p.IdPapelGrupo == 1 && p.Ativo == 1)
                .OrderBy(p => p.Nome)
                .Select(p => new { p.Id, p.Nome, p.Cpf, p.IsentoPagamento }) // ALTERAÇÃO: Adicionado o campo "Isento"
                .ToListAsync();

            // Busca os pagamentos já existentes para esta receita
            var pagamentos = await _context.Receitafinanceirapessoas
                .Where(p => p.IdReceitaFinanceira == idReceita)
                .ToDictionaryAsync(p => p.IdPessoa);

            var result = new List<AssociadoPagamentoDTO>();
            // Monta a lista final de DTOs
            foreach (var associado in associadosDoGrupo)
            {
                var dto = new AssociadoPagamentoDTO
                {
                    IdAssociado = associado.Id,
                    NomeAssociado = associado.Nome,
                    Cpf = associado.Cpf
                };

                // Lógica para definir o status do pagamento
                // 1. Se um pagamento para este associado for encontrado, atualiza os dados
                if (pagamentos.TryGetValue(associado.Id, out var pagamento))
                {
                    dto.DataPagamento = pagamento.DataPagamento;
                    dto.ValorPago = pagamento.Valor;
                    dto.Observacoes = pagamento.Observacoes;
                    dto.Status = pagamento.Status;
                }
                // 2. Se não houver pagamento e o associado for isento, define o status como ISENTO
                else if (associado.IsentoPagamento == 1)
                {
                    dto.Status = StatusPagamento.ISENTO;
                }
                // 3. Caso contrário, o status padrão é NAO_PAGOU
                else
                {
                    dto.Status = StatusPagamento.NAO_PAGOU;
                }

                result.Add(dto);
            }

            return result;
        }


        public async Task SalvarPagamentos(int idReceita, IEnumerable<AssociadoPagamentoDTO> associados)
        {
            var pagamentosExistentes = await _context.Receitafinanceirapessoas
                .Where(p => p.IdReceitaFinanceira == idReceita)
                .ToListAsync();

            foreach (var associadoPagamento in associados)
            {
                var pagamento = pagamentosExistentes.FirstOrDefault(p => p.IdPessoa == associadoPagamento.IdAssociado);

                if (associadoPagamento.Status == "NAO_PAGOU")
                {
                    if (pagamento != null)
                    {
                        _context.Receitafinanceirapessoas.Remove(pagamento);
                    }
                    continue;
                }

                if (pagamento == null)
                {
                    pagamento = new Receitafinanceirapessoa
                    {
                        IdPessoa = associadoPagamento.IdAssociado,
                        IdReceitaFinanceira = idReceita
                    };
                    _context.Receitafinanceirapessoas.Add(pagamento);
                }

                pagamento.DataPagamento = associadoPagamento.DataPagamento;
                pagamento.Valor = associadoPagamento.ValorPago;
                pagamento.Observacoes = associadoPagamento.Observacoes;
                pagamento.Status = associadoPagamento.Status;
            }

            await _context.SaveChangesAsync();
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