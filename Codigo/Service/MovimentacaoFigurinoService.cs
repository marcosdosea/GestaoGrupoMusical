using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MovimentacaoFigurinoService : IMovimentacaoFigurinoService
    {
        private readonly GrupoMusicalContext _context;

        public MovimentacaoFigurinoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        //emprestar figurino
        public async Task<int> CreateAsync(Movimentacaofigurino movimentacao)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var figurinoEstoque = await _context.Figurinomanequims.FindAsync(movimentacao.IdFigurino, movimentacao.IdManequim);

                if (figurinoEstoque != null)
                {
                    if (figurinoEstoque.QuantidadeDisponivel == 0)
                    {
                        return 401; //não há peças disponiveis para emprestar
                    }
                }
                else
                {
                    return 400; //estoque nao existe, talvez id esteja errado
                }

                movimentacao.Status = "EMPRESTADO";
                movimentacao.ConfirmacaoRecebimento = 0;

                await _context.AddAsync(movimentacao);

                figurinoEstoque.QuantidadeDisponivel--;

                _context.Figurinomanequims.Update(figurinoEstoque);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var associado = await _context.Pessoas.FindAsync(movimentacao.IdAssociado);

                string? figurinoNome = (await _context.Figurinos.FindAsync(movimentacao.IdFigurino))?.Nome;

                if (associado != null)
                {
                    EmailModel email = new()
                    {
                        Assunto = "Batalá - Empréstimo de Figurino",
                        AddresseeName = associado.Nome,
                        Body = "<div style=\"text-align: center;\">\r\n    " +
                        $"<h3>Estamos aguardando a sua confirmação de Empréstimo.</h3>\r\n" +
                        "<div style=\"font-size: large;\">\r\n        " +
                        $"<dt style=\"font-weight: 700;\">Instrumento:</dt><dd>{figurinoNome}</dd>" +
                        $"<dt style=\"font-weight: 700;\">Data de Emprestimo:</dt><dd>{movimentacao.Data:dd/MM/yyyy}</dd>\n</div>"
                    };

                    email.To.Add(associado.Email);

                    await EmailService.Enviar(email);
                }


            }
            catch
            {
                await transaction.RollbackAsync();
                return 500;
            }

            return 200;
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MovimentacaoFigurinoDTO>> GetAllByIdInstrumento(int idFigurino)
        {
            throw new NotImplementedException();
        }

        public Task<Movimentacaofigurino?> GetEmprestimoByIdInstrumento(int idFigurino)
        {
            throw new NotImplementedException();
        }
    }
}
