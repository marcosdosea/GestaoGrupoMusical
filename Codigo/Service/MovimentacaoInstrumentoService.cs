using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class MovimentacaoInstrumentoService : IMovimentacaoInstrumentoService
    {
        GrupoMusicalContext _context;

        public MovimentacaoInstrumentoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(Movimentacaoinstrumento movimentacao)
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

                        return true;
                    }
                    else if(movimentacao.TipoMovimento == "DEVOLUCAO" && instrumento.Status == "EMPRESTADO")
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

                        return true;
                    }
                }
                
                await transaction.RollbackAsync();
                return false;
                
            }
            catch 
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(id);
                if (movimentacao != null)
                {
                    _context.Movimentacaoinstrumentos.Remove(movimentacao);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
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
                        select movimentacao).AsNoTracking().FirstOrDefaultAsync();

            return await query;
        }

        public async Task<bool> NotificarViaEmail(int id)
        {
            try
            {
                var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(id);
                if (movimentacao != null)
                {
                    var instrumento = await _context.Instrumentomusicals.FindAsync(movimentacao.IdInstrumentoMusical);
                    var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);
                    string tipoMovimentacao = movimentacao.TipoMovimento == "DEVOLUCAO" ? "Devolução" : "Empréstimo";

                    if (associado != null && instrumento != null)
                    {
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

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
