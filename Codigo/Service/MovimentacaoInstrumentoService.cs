using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using static Core.DTO.InstrumentoAssociadoDTO;

namespace Service
{
    public class MovimentacaoInstrumentoService : IMovimentacaoInstrumentoService
    {
        GrupoMusicalContext _context;

        public MovimentacaoInstrumentoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Movimentacaoinstrumento movimentacao)
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
                                Body = "<div style=\"text-align: center;\">\r\n    " +
                                "<h1>Empréstimo de instrumento</h1>\r\n    " +
                                $"<h2>Olá, {associado.Nome}, estamos aguardando a sua confirmação de recebimento.</h2>\r\n" +
                                "<div style=\"font-size: large;\">\r\n        " +
                                $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                $"<dt style=\"font-weight: 700;\">Data de Emprestimo:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                            };

                            email.To.Add(associado.Email);

                            await EmailService.Enviar(email);
                        }

                        return 200;
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
                                    Body = "<div style=\"text-align: center;\">\r\n    " +
                                    "<h1>Devolução de instrumento</h1>\r\n    " +
                                    $"<h2>Olá, {associado.Nome}, estamos aguardando a sua confirmação de devolução.</h2>\r\n" +
                                    "<div style=\"font-size: large;\">\r\n        " +
                                    $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                    $"<dt style=\"font-weight: 700;\">Data de Devolução:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                                };

                                email.To.Add(associado.Email);

                                await EmailService.Enviar(email);
                            }

                            return 200;
                        }
                        await transaction.RollbackAsync();
                        return 402;
                    }
                    await transaction.RollbackAsync();
                    return 401;
                }
                
                await transaction.RollbackAsync();
                return 400;
                
            }
            catch 
            {
                await transaction.RollbackAsync();
                return 500;
            }
        }

        public async Task<int> DeleteAsync(int id)
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
                            return 400;
                        }
                         _context.Movimentacaoinstrumentos.Remove(movimentacao);
                        await _context.SaveChangesAsync();
                        return 200;
                    }
                }
                return 404;
            }
            catch
            {
                return 500;
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

        public async Task<int> ConfirmarMovimentacaoAsync(int idMovimentacao, int idAssociado)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(idMovimentacao);

                if(movimentacao == null)
                {
                    return 404;
                }

                if(movimentacao.IdAssociado != idAssociado)
                {
                    return movimentacao.TipoMovimento == "DEVOLUCAO" ? 401 : 400;
                }

                movimentacao.ConfirmacaoAssociado = 1;

                _context.Update(movimentacao);
                await _context.SaveChangesAsync();

                return movimentacao.TipoMovimento == "DEVOLUCAO" ? 201 : 200;
            }
            catch
            {
                return 500;
            }
        }

        public async Task<int> NotificarViaEmailAsync(int id)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(id);
                if (movimentacao != null)
                {
                    var instrumento = await _context.Instrumentomusicals.FindAsync(movimentacao.IdInstrumentoMusical);
                    var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);
                    string tipoMovimentacao = movimentacao.TipoMovimento == "DEVOLUCAO" ? "Devolução" : "Empréstimo";

                    if(associado == null)
                    {
                        return 402;
                    }

                    if(instrumento == null)
                    {
                        return 401;
                    }

                    string? instrumentoNome = (await _context.Tipoinstrumentos.FindAsync(instrumento.IdTipoInstrumento))?.Nome;

                    EmailModel email = new()
                    {
                        Assunto = $"Batalá - {tipoMovimentacao} de instrumento",
                        Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h1>{tipoMovimentacao} de instrumento</h1>\r\n    " +
                                $"<h2>Olá, {associado.Nome}, estamos aguardando a sua confirmação de {tipoMovimentacao}.</h2>\r\n" +
                                "<div style=\"font-size: large;\">\r\n        " +
                                $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{instrumentoNome}</dd>" +
                                $"<dt style=\"font-weight: 700;\">Data de {tipoMovimentacao}:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                    };

                    email.To.Add(associado.Email);

                    await EmailService.Enviar(email);

                    return 200;
                    
                }

                return 404;
            }
            catch
            {
                return 500;
            }
        }
    }
}
