using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Core.DTO.InstrumentoAssociadoDTO;

namespace Service
{
    public class MovimentacaoInstrumentoService : IMovimentacaoInstrumentoService
    {
        private readonly GrupoMusicalContext _context;

        public MovimentacaoInstrumentoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> CreateAsync(Movimentacaoinstrumento movimentacao)
        {
            using var transaction = _context.Database.BeginTransaction();

            try 
            {
                movimentacao.ConfirmacaoAssociado = 0;
                await _context.AddAsync(movimentacao);

                var instrumento = await _context.Instrumentomusicals.FindAsync(movimentacao.IdInstrumentoMusical);
                if (instrumento != null && instrumento.Status != "DANIFICADO")
                {
                    if(movimentacao.TipoMovimento == "EMPRESTIMO" && instrumento.Status == "DISPONIVEL")
                    {
                        instrumento.Status = "EMPRESTADO";
                        _context.Instrumentomusicals.Update(instrumento);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);
                        string? instrumentoNome = (await _context.Tipoinstrumentos.FindAsync(instrumento.IdTipoInstrumento))?.Nome;
                        if (associado != null)
                        {
                            EmailModel email = new()
                            {
                                Assunto = "Batalá - Empréstimo de instrumento",
                                AddresseeName = associado.Nome,
                                Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h3>Estamos aguardando a sua confirmação de Empréstimo.</h3>\r\n" +
                                "<div style=\"font-size: large;\">\r\n        " +
                                $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                $"<dt style=\"font-weight: 700;\">Data de Emprestimo:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                            };

                            email.To.Add(associado.Email);

                            await EmailService.Enviar(email);
                        }

                        return HttpStatusCode.Created;
                    }
                    else if(movimentacao.TipoMovimento == "DEVOLUCAO" && instrumento.Status == "EMPRESTADO")
                    {
                        var emprestimo = await GetEmprestimoByIdInstrumento(instrumento.Id);

                        if (emprestimo != null && movimentacao.IdAssociado == emprestimo.IdAssociado)
                        {
                            instrumento.Status = "DISPONIVEL";
                            _context.Instrumentomusicals.Update(instrumento);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);
                            string? instrumentoNome = (await _context.Tipoinstrumentos.FindAsync(instrumento.IdTipoInstrumento))?.Nome;
                            if (associado != null)
                            {
                                EmailModel email = new()
                                {
                                    Assunto = "Batalá - Devolução de instrumento",
                                    AddresseeName = associado.Nome,
                                    Body = "<div style=\"text-align: center;\">\r\n    " +
                                    $"<h3>Estamos aguardando a sua confirmação de Devolução.</h3>\r\n" +
                                    "<div style=\"font-size: large;\">\r\n        " +
                                    $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                    $"<dt style=\"font-weight: 700;\">Data de Devolução:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                                };

                                email.To.Add(associado.Email);

                                await EmailService.Enviar(email);
                            }

                            return HttpStatusCode.Created;
                        }
                        await transaction.RollbackAsync();
                        return HttpStatusCode.PreconditionFailed;
                    }
                    await transaction.RollbackAsync();
                    return HttpStatusCode.Conflict;
                }
                
                await transaction.RollbackAsync();
                return HttpStatusCode.BadRequest;
                
            }
            catch 
            {
                await transaction.RollbackAsync();
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(int id)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(id);
                if (movimentacao != null)
                {
                    var movimentacaoDb = await GetEmprestimoByIdInstrumento(movimentacao.IdInstrumentoMusical);
                    if (movimentacaoDb != null)
                    {
                        if (movimentacaoDb.Id == movimentacao.Id)
                        {
                            return HttpStatusCode.PreconditionFailed;
                        }
                         _context.Movimentacaoinstrumentos.Remove(movimentacao);
                        await _context.SaveChangesAsync();
                        return HttpStatusCode.OK;
                    }
                }
                return HttpStatusCode.NotFound;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<IEnumerable<MovimentacaoInstrumentoDTO>> GetAllByIdInstrumento(int idInstrumento)
        {
            var query = await   (from movimentacao in _context.Movimentacaoinstrumentos
                                 where movimentacao.IdInstrumentoMusical == idInstrumento
                                orderby movimentacao.Data descending
                                select new MovimentacaoInstrumentoDTO
                                {
                                    Id = movimentacao.Id,
                                    IdInstrumento = movimentacao.IdInstrumentoMusical,
                                    Cpf = movimentacao.IdAssociadoNavigation.Cpf,
                                    NomeAssociado = movimentacao.IdAssociadoNavigation.Nome,
                                    Data = movimentacao.Data,
                                    Movimentacao = movimentacao.TipoMovimento,
                                    Status = movimentacao.ConfirmacaoAssociado == 0 ? "Aguardando Confirmação" : "Confirmado"
                                }).AsNoTracking().ToListAsync();

            foreach (var movimentacao in query)
            {
                if(movimentacao.Movimentacao == "DEVOLUCAO")
                {
                    movimentacao.Movimentacao = "Devolução";
                }
                else
                {
                    movimentacao.Movimentacao = "Empréstimo";
                }
            }

            return query;
        }

        public async Task<Movimentacaoinstrumento?> GetEmprestimoByIdInstrumento(int idInstrumento)
        {
            var query = (from movimentacao in _context.Movimentacaoinstrumentos
                        where movimentacao.IdInstrumentoMusical == idInstrumento
                        where movimentacao.TipoMovimento == "EMPRESTIMO"
                        orderby movimentacao.Data descending
                        select movimentacao).AsNoTracking().FirstOrDefaultAsync();

            return await query;
        }

        public async Task<MovimentacoesAssociado> MovimentacoesByIdAssociadoAsync(int idAssociado)
        {
            var devolucao = (from movimentacao in _context.Movimentacaoinstrumentos
                             where movimentacao.IdAssociado == idAssociado
                             where movimentacao.TipoMovimento == "DEVOLUCAO"
                             orderby movimentacao.Data descending
                             select new MovimentacaoAssociado
                             {
                                 Id = movimentacao.Id,
                                 Data = movimentacao.Data,
                                 NomeInstrumento = movimentacao.IdInstrumentoMusicalNavigation.IdTipoInstrumentoNavigation.Nome,
                                 NomeStatus = movimentacao.ConfirmacaoAssociado == 1 ? "Confirmado" : "Aguardando Confirmação",
                                 Status = movimentacao.ConfirmacaoAssociado == 1
                             }
                            ).AsNoTracking();

            var emprestimo = (from movimentacao in _context.Movimentacaoinstrumentos
                             where movimentacao.IdAssociado == idAssociado
                             where movimentacao.TipoMovimento == "EMPRESTIMO"
                             orderby movimentacao.Data descending
                             select new MovimentacaoAssociado
                             {
                                 Id = movimentacao.Id,
                                 Data = movimentacao.Data,
                                 NomeInstrumento = movimentacao.IdInstrumentoMusicalNavigation.IdTipoInstrumentoNavigation.Nome,
                                 NomeStatus = movimentacao.ConfirmacaoAssociado == 1 ? "Confirmado" : "Aguardando Confirmação",
                                 Status = movimentacao.ConfirmacaoAssociado == 1
                             }
                            ).AsNoTracking();

            MovimentacoesAssociado movimentacoes = new()
            {
                Devolucoes = await devolucao.ToListAsync(),
                Emprestimos = await emprestimo.ToListAsync(),
            };

            return movimentacoes;
        }

        public async Task<HttpStatusCode> ConfirmarMovimentacaoAsync(int idMovimentacao, int idAssociado)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(idMovimentacao);

                if(movimentacao == null)
                {
                    return HttpStatusCode.NotFound;
                }

                if(movimentacao.IdAssociado != idAssociado)
                {
                    return movimentacao.TipoMovimento == "DEVOLUCAO" ? HttpStatusCode.BadRequest : HttpStatusCode.PreconditionFailed;
                }

                movimentacao.ConfirmacaoAssociado = 1;

                _context.Update(movimentacao);
                await _context.SaveChangesAsync();

                return movimentacao.TipoMovimento == "DEVOLUCAO" ? HttpStatusCode.OK : HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> NotificarViaEmailAsync(int id)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(id);
                if (movimentacao != null)
                {
                    if (Convert.ToBoolean(movimentacao.ConfirmacaoAssociado))
                    {
                        return movimentacao.TipoMovimento == "DEVOLUCAO" ? HttpStatusCode.BadGateway : HttpStatusCode.BadRequest;
                    }
                    var instrumento = await _context.Instrumentomusicals.FindAsync(movimentacao.IdInstrumentoMusical);
                    var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);
                    string tipoMovimentacao = movimentacao.TipoMovimento == "DEVOLUCAO" ? "Devolução" : "Empréstimo";

                    if(associado == null)
                    {
                        return HttpStatusCode.PreconditionRequired;
                    }

                    if(instrumento == null)
                    {
                        return HttpStatusCode.PreconditionFailed;
                    }

                    string? instrumentoNome = (await _context.Tipoinstrumentos.FindAsync(instrumento.IdTipoInstrumento))?.Nome;

                    EmailModel email = new()
                    {
                        Assunto = $"Batalá - {tipoMovimentacao} de instrumento",
                        AddresseeName = associado.Nome,
                        Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h3>Estamos aguardando a sua confirmação de {tipoMovimentacao}.</h3>\r\n" +
                                "<div style=\"font-size: large;\">\r\n        " +
                                $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                $"<dt style=\"font-weight: 700;\">Data de {tipoMovimentacao}:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                    };

                    email.To.Add(associado.Email);

                    await EmailService.Enviar(email);

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
