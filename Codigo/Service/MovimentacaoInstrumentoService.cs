using Core;
using Core.DTO;
using Core.Service;
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
                        return true;
                    }
                    else if(movimentacao.TipoMovimento == "DEVOLUCAO" && instrumento.Status == "EMPRESTADO")
                    {
                        instrumento.Status = "DISPONIVEL";
                        _context.Instrumentomusicals.Update(instrumento);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
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

        public async Task<IEnumerable<MovimentacaoInstrumentoDTO>> GetAll()
        {
            var query = await   (from movimentacao in _context.Movimentacaoinstrumentos
                                orderby movimentacao.Data descending
                                select new MovimentacaoInstrumentoDTO
                                {
                                    Id = movimentacao.Id,
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
    }
}
