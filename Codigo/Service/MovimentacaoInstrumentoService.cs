using Core;
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
                if (instrumento == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                else
                {
                    instrumento.Status = movimentacao.TipoMovimento == "EMPRESTIMO" ? "EMPRESTADO" : "DISPONIVEL";
                    _context.Instrumentomusicals.Update(instrumento);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
            }
            catch 
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
