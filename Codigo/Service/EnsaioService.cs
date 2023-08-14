﻿using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class EnsaioService : IEnsaioService
    {
        private readonly GrupoMusicalContext _context;
        public EnsaioService(GrupoMusicalContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Cadastra um novo Ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se cadastrou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<int> Create(Ensaio ensaio)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if(ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        await _context.Ensaios.AddAsync(ensaio);

                        var associados = _context.Pessoas
                                         .Where(p => p.IdPapelGrupo == 1 && p.Ativo == 1 && p.IdGrupoMusical == ensaio.IdGrupoMusical)
                                         .Select(pessoa => new { pessoa.Id, pessoa.Email }).AsNoTracking();

                        await _context.SaveChangesAsync();

                        EmailModel email = new()
                        {
                            Assunto = "Batalá - Novo Ensaio Cadastrado",
                            AddresseeName = "Associado",
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                    $"<h3>Um novo ensaio foi cadastrado.</h3>\r\n" +
                                    "<div style=\"font-size: large;\">\r\n        " +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Início:</dt><dd>{ensaio.DataHoraInicio}</dd>" +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Fim:</dt><dd>{ensaio.DataHoraFim}</dd>\n</div>"
                        };

                        await associados.ForEachAsync(async associado => {
                            Ensaiopessoa ensaioPessoa = new()
                            {
                                IdEnsaio = ensaio.Id,
                                IdPessoa = associado.Id,
                                Presente = 1
                            };
                            await _context.Ensaiopessoas.AddAsync(ensaioPessoa);
                            email.To.Add(associado.Email);
                        });

                        await EmailService.Enviar(email);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return 200;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return 400;
                    }
                   
                }
                else 
                {
                    await transaction.RollbackAsync();
                    return 401;
                }
             
            }
            catch
            {
                await transaction.RollbackAsync();
                return 500;
            }
        }
        /// <summary>
        /// Deleta um Ensaio do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se deletou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                _context.Ensaios.Remove(await Get(id));
                await _context.SaveChangesAsync();
                return 200;
            }
            catch
            {
                return 500;
            }
        }
        /// <summary>
        /// Edita um Ensaio do banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>retorna um inteiro.</returns>
        public async Task<int> Edit(Ensaio ensaio)
        {

             try
            {
                var ensaioDb = await _context.Ensaios.Where(e => e.Id == ensaio.Id).AsNoTracking().SingleOrDefaultAsync();
                if(ensaioDb != null)
                {
                    ensaio.IdColaboradorResponsavel = ensaioDb.IdColaboradorResponsavel;
                    ensaio.IdGrupoMusical = ensaioDb.IdGrupoMusical;
                }
                _context.Ensaios.Update(ensaio);
                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if(ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        await _context.SaveChangesAsync();
                        return 200;
                    }
                    else
                    {
                        return 400;
                    }
                   
                }
                else 
                {
                    return 401;
                }
             
            }
            catch
            {
                return 500;
            }
        }
        /// <summary>
        /// Consulta um Ensaio no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Ensaio correspondente ao id ou um Ensaio vazio</returns>
        public async Task<Ensaio> Get(int id)
        {
            return await _context.Ensaios.FindAsync(id) ?? new Ensaio();
        }
        /// <summary>
        /// Consulta todos os Ensaios no banco de dados
        /// </summary>
        /// <returns>Uma lista contendo todos os Ensaios</returns>
        public async Task<IEnumerable<Ensaio>> GetAll()
        {
            return await _context.Ensaios.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<EnsaioDTO>> GetAllDTO()
        {
            var query = _context.Ensaios
                .OrderBy(g => g.DataHoraInicio)
                .Select(g =>
                new EnsaioDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local
                }).AsNoTracking().ToListAsync();
            return await query;
        }

        public async Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO()
        {
            var query = _context.Ensaios
                .OrderBy(g => g.DataHoraInicio)
                .Select(g => new EnsaioIndexDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Tipo = g.Tipo,
                    Local = g.Local,
                    PresencaObrigatoria = g.PresencaObrigatoria

                }).AsNoTracking().ToListAsync();
            return await query;
        }

        public async Task<IEnumerable<EnsaioFrequenciaDTO>> GetFrequencia(int idEnsaio, int idGrupoMusical)
        {
            var query = from ensaioPessoa in _context.Ensaiopessoas
                        where ensaioPessoa.IdEnsaio == idEnsaio && ensaioPessoa.IdEnsaioNavigation.IdGrupoMusical == idGrupoMusical
                        orderby ensaioPessoa.IdPessoaNavigation.Nome
                        select new EnsaioFrequenciaDTO
                        {
                            ensaioPessoa.IdPessoaNavigation.Cpf
                        };

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
