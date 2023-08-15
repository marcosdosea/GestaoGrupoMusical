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
using static Core.DTO.MovimentacaoAssociadoFigurinoDTO;

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
                        await transaction.RollbackAsync();
                        return 401; //não há peças disponiveis para emprestar
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return 400; //estoque nao existe, talvez id esteja errado
                }

                await _context.AddAsync(movimentacao);

                if (movimentacao.Status.Equals("ENTREGUE"))
                {
                    figurinoEstoque.QuantidadeDisponivel--;
                    figurinoEstoque.QuantidadeEntregue++;
                }
                else if (movimentacao.Status.Equals("DEVOLVIDO") || movimentacao.Status.Equals("DANIFICADO"))
                {
                    if (await AssociadoEmprestimo(movimentacao.IdAssociado, movimentacao.IdFigurino, movimentacao.IdManequim))
                    {
                        await transaction.RollbackAsync();
                        return 402; //associado nao possue nada emprestado para devolver
                    }
                    if (movimentacao.Status.Equals("DEVOLVIDO"))
                    {
                        figurinoEstoque.QuantidadeDisponivel++;
                        figurinoEstoque.QuantidadeEntregue--;
                    }
                }


                _context.Figurinomanequims.Update(figurinoEstoque);

                await _context.SaveChangesAsync();

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
                        $"<dt style=\"font-weight: 700;\">Figurino:</dt><dd>{figurinoNome}</dd>" +
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

            await transaction.CommitAsync();
            return 200;
        }


        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MovimentacaoFigurinoDTO>> GetAllByIdFigurino(int idFigurino)
        {
            var query = await (from movimentacoes in _context.Movimentacaofigurinos
                               where movimentacoes.IdFigurino == idFigurino
                               orderby movimentacoes.Id descending
                               select new MovimentacaoFigurinoDTO
                               {
                                   Id = movimentacoes.Id,
                                   IdFigurino = idFigurino,
                                   IdManequim = movimentacoes.IdManequim,
                                   Cpf = movimentacoes.IdAssociadoNavigation.Cpf,
                                   NomeAssociado = movimentacoes.IdAssociadoNavigation.Nome,
                                   Data = movimentacoes.Data,
                                   Movimentacao = movimentacoes.Status,
                                   Status = movimentacoes.ConfirmacaoRecebimento == 0 ? "Aguardando Confirmação" : "Confirmado",
                                   Tamanho = movimentacoes.IdManequimNavigation.Tamanho
                               }).AsNoTracking().ToListAsync();

            return query;
        }


        public Task<Movimentacaofigurino?> GetEmprestimoByIdFigurino(int idFigurino)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AssociadoEmprestimo(int idAssociado, int idFigurino, int idManequim)
        {
            int devolvidos = await (from movimentacoes in _context.Movimentacaofigurinos
                                    where movimentacoes.IdAssociado == idAssociado &&
                                    movimentacoes.IdFigurino == idFigurino &&
                                    movimentacoes.IdManequim == idManequim &&
                                    (movimentacoes.Status == "DEVOLVIDO" || movimentacoes.Status == "DANIFICADO")
                                    select movimentacoes
                                    ).AsNoTracking().CountAsync();

            int recebidos = await (from movimentacoes in _context.Movimentacaofigurinos
                                   where movimentacoes.IdAssociado == idAssociado &&
                                   movimentacoes.IdFigurino == idFigurino &&
                                   movimentacoes.IdManequim == idManequim &&
                                   (movimentacoes.Status == "ENTREGUE" || movimentacoes.Status == "RECEBIDO")
                                   select movimentacoes
                                    ).AsNoTracking().CountAsync();

            return (recebidos - devolvidos) <= 0 ? true : false;
        }

        public async Task<IEnumerable<EstoqueDTO>> GetEstoque(int idFigurino)
        {
            var query = await (from estoque in _context.Figurinomanequims
                               where estoque.IdFigurino == idFigurino
                               select new EstoqueDTO
                               {
                                   IdFigurino = estoque.IdFigurino,
                                   IdManequim = estoque.IdManequim,
                                   Tamanho = estoque.IdManequimNavigation.Tamanho,
                                   Disponivel = estoque.QuantidadeDisponivel,
                                   Entregues = estoque.QuantidadeEntregue
                               }
                         ).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<MovimentacoesAssociadoFigurino> MovimentacoesByIdAssociadoAsync(int idAssociado)
        {
            var entregues = await (from movimentacoesFigurino in _context.Movimentacaofigurinos
                             where movimentacoesFigurino.IdAssociado == idAssociado
                             where movimentacoesFigurino.Status == "ENTREGUE" || movimentacoesFigurino.Status == "RECEBIDO"
                                   orderby movimentacoesFigurino.Data descending
                             select new MovimentacaoAssociadoFigurino
                             {
                                 Id = movimentacoesFigurino.Id,
                                 Data = movimentacoesFigurino.Data,
                                 NomeFigurino = movimentacoesFigurino.IdFigurinoNavigation.Nome,
                                 Tamanho = movimentacoesFigurino.IdManequimNavigation.Tamanho,
                                 Status = movimentacoesFigurino.ConfirmacaoRecebimento == 1 ? "Confirmado" : "Agurdando Confirmação"

                             }).AsNoTracking().ToListAsync();

            var devolucoes = await (from movimentacoesFigurino in _context.Movimentacaofigurinos
                              where movimentacoesFigurino.IdAssociado == idAssociado
                              where movimentacoesFigurino.Status == "DEVOLVIDO" || movimentacoesFigurino.Status == "DANIFICADO"
                                    orderby movimentacoesFigurino.Data descending
                              select new MovimentacaoAssociadoFigurino
                              {
                                  Id = movimentacoesFigurino.Id,
                                  Data = movimentacoesFigurino.Data,
                                  NomeFigurino = movimentacoesFigurino.IdFigurinoNavigation.Nome,
                                  Tamanho = movimentacoesFigurino.IdManequimNavigation.Tamanho,
                                  Status = movimentacoesFigurino.ConfirmacaoRecebimento == 1 ? "Confirmado" : "Agurdando Confirmação"
                              }

                              ).AsNoTracking().ToListAsync();



            var movimentacoes = new MovimentacoesAssociadoFigurino
            {
                Entregue =entregues,
                Devolucoes = devolucoes
            };

            return movimentacoes;
        }

        public async Task<int> ConfirmarMovimentacao(int idMovimentacao, int idAssociado)
        {
            try
            {
                var movimentacao = await _context.Movimentacaofigurinos.FindAsync(idMovimentacao);
                if(movimentacao == null)
                {
                    return 404;
                }else if(movimentacao.IdAssociado == idAssociado && movimentacao.Id == idMovimentacao)
                {
                    movimentacao.ConfirmacaoRecebimento = 1;
                    if (movimentacao.Status.Equals("ENTREGUE"))
                    {
                        movimentacao.Status = "RECEBIDO";
                    }
                    _context.Update(movimentacao);
                    await _context.SaveChangesAsync();
                    return movimentacao.Status == "ENTREGUE" ? 200 : 201;
                }
                else
                {
                    return movimentacao.Status == "ENTREGUE" ? 400 : 401;
                }       
            }
            catch
            {
                return 500;
            }
        }
    }
}
