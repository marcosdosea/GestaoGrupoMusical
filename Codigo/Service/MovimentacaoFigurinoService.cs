using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
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
        public async Task<HttpStatusCode> CreateAsync(Movimentacaofigurino movimentacao)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var figurinoEstoque = await _context.Figurinomanequims.FindAsync(movimentacao.IdFigurino, movimentacao.IdManequim);

                if (figurinoEstoque != null)
                {
                    if (figurinoEstoque.QuantidadeDisponivel == 0 && movimentacao.Status.Equals("ENTREGUE"))
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

                if (movimentacao.Status.Equals("ENTREGUE"))
                {
                    if (movimentacao.Quantidade <= 0 || movimentacao.Quantidade >
                        figurinoEstoque.QuantidadeDisponivel)
                    {
                        await transaction.RollbackAsync();
                        return 401; //não há peças disponiveis para emprestar
                    }
                    figurinoEstoque.QuantidadeDisponivel -= movimentacao.Quantidade;
                    figurinoEstoque.QuantidadeEntregue+= movimentacao.Quantidade;
                    await _context.AddAsync(movimentacao);
                }
                else if (movimentacao.Status.Equals("DEVOLVIDO"))
                {
                    if (await AssociadoEmprestimo(movimentacao.IdAssociado, movimentacao.IdFigurino, movimentacao.IdManequim))
                    {
                        await transaction.RollbackAsync();
                        return 402; //associado nao possue nada emprestado para devolver
                    }
                    if (movimentacao.Status.Equals("DEVOLVIDO"))
                    {
                        var confiQuantAssociado = await GetConfirmacaoFigurino(movimentacao.IdAssociado, movimentacao.IdFigurino
                            , movimentacao.IdManequim);
                        if (confiQuantAssociado.Confirmar != 1)
                        {
                            await transaction.RollbackAsync();
                            return 403; //não houve confirmação
                        }
                        if (movimentacao.Quantidade <= 0 || movimentacao.Quantidade > confiQuantAssociado.Quantidade)
                        {
                            await transaction.RollbackAsync();
                            return 403; //tentativa de devolução de figurino a mais ou a menos da quantidade que o associado possui
                        }
                        else
                        {
                            if (movimentacao.Quantidade < confiQuantAssociado.Quantidade)
                            {
                                int movQuantidade = confiQuantAssociado.Quantidade - movimentacao.Quantidade;

                                var movimentacaoRecebido = new Movimentacaofigurino
                                {
                                    Data = DateTime.Now,
                                    IdFigurino = movimentacao.IdFigurino,
                                    IdManequim = movimentacao.IdManequim,
                                    IdAssociado = movimentacao.IdAssociado,
                                    IdColaborador = movimentacao.IdColaborador,
                                    Status = "RECEBIDO",
                                    ConfirmacaoRecebimento = 1,
                                    Quantidade = movQuantidade
                                };


                                var movimentacaoDevolvido = new Movimentacaofigurino
                                {
                                    Data = DateTime.Now,
                                    IdFigurino = movimentacao.IdFigurino,
                                    IdManequim = movimentacao.IdManequim,
                                    IdAssociado = movimentacao.IdAssociado,
                                    IdColaborador = movimentacao.IdColaborador,
                                    Status = "DEVOLVIDO",
                                    ConfirmacaoRecebimento = 0,
                                    Quantidade = movimentacao.Quantidade
                                };

                                await _context.AddAsync(movimentacaoDevolvido);
                                await _context.AddAsync(movimentacaoRecebido);

                                figurinoEstoque.QuantidadeDisponivel += movimentacao.Quantidade;
                                figurinoEstoque.QuantidadeEntregue -= movimentacao.Quantidade;

                            }
                            else
                            {
                                movimentacao.ConfirmacaoRecebimento = 0;
                                figurinoEstoque.QuantidadeDisponivel += confiQuantAssociado.Quantidade;
                                figurinoEstoque.QuantidadeEntregue -= confiQuantAssociado.Quantidade;
                                await _context.AddAsync(movimentacao);
                            }
                        }

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

        public async Task<HttpStatusCode> DeleteAsync(int id)
        {
            var movimentacao = await _context.Movimentacaofigurinos.FindAsync(id);

            if (movimentacao != null)
            {
                try
                {
                    _context.Movimentacaofigurinos.Remove(movimentacao);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return 500; //algo deu errado ao remover e/ou salvar
                }

            }
            else
            {
                return 400; //movimentacao nao existe
            }

            return 200;
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
                                   Tamanho = movimentacoes.IdManequimNavigation.Tamanho,
                                   QuantidadeEntregue = movimentacoes.Quantidade,

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
                                       Status = movimentacoesFigurino.ConfirmacaoRecebimento == 1 ? "Confirmado" : "Agurdando Confirmação",
                                       Quantidade = movimentacoesFigurino.Quantidade

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
                                        Status = movimentacoesFigurino.ConfirmacaoRecebimento == 1 ? "Confirmado" : "Agurdando Confirmação",
                                        Quantidade = movimentacoesFigurino.Quantidade
                                    }

                              ).AsNoTracking().ToListAsync();



            var movimentacoes = new MovimentacoesAssociadoFigurino
            {
                Entregue = entregues,
                Devolucoes = devolucoes
            };

            return movimentacoes;
        }

        public async Task<HttpStatusCode> ConfirmarMovimentacao(int idMovimentacao, int idAssociado)
        {
            try
            {
                var movimentacao = await _context.Movimentacaofigurinos.FindAsync(idMovimentacao);
                if (movimentacao == null)
                {
                    return 404;
                }
                else if (movimentacao.IdAssociado == idAssociado && movimentacao.Id == idMovimentacao)
                {
                    movimentacao.ConfirmacaoRecebimento = 1;
                    var status = movimentacao.Status;
                    if (movimentacao.Status.Equals("ENTREGUE"))
                    {
                        movimentacao.Status = "RECEBIDO";
                    }
                    _context.Update(movimentacao);
                    await _context.SaveChangesAsync();
                    return status == "ENTREGUE" ? 200 : 201;
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

        public async Task<MovimentarConfirmacaoQuantidade> GetConfirmacaoFigurino(int idAssociado, int idFigurino, int idManequim)
        {
            var query = await _context.Movimentacaofigurinos
                .AsNoTracking()
                .Where(g => g.IdAssociado == idAssociado && g.IdFigurino == idFigurino
                    && g.IdManequim == idManequim)
                .OrderBy(g => g.Id)
                .Select(g => new MovimentarConfirmacaoQuantidade
                {
                    Confirmar = g.ConfirmacaoRecebimento,
                    Quantidade = g.Quantidade,
                    Id = g.Id

                })
                .LastOrDefaultAsync();

            return query;
        }
    }
}
