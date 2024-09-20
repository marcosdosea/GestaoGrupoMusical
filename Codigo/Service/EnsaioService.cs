﻿using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

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
        public async Task<HttpStatusCode> Create(Ensaio ensaio, IEnumerable<int> idRegentes, int idFigurino)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if (ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        await _context.Ensaios.AddAsync(ensaio);
                        await _context.SaveChangesAsync();

                        List<Ensaiopessoa> p = new();
                        foreach (int id in idRegentes)
                        {
                            p.Add(new Ensaiopessoa
                            {
                                IdEnsaio = ensaio.Id,
                                IdPessoa = id,
                                IdPapelGrupo = 5 //5 significa que seja um Regente!
                            });
                        }
                        _context.Ensaiopessoas.AddRange(p);
                        _context.SaveChanges();
                        _context.Set<Dictionary<string, object>>("Figurinoensaio").Add(new Dictionary<string, object>
                        {
                            {"IdFigurino", idFigurino },
                            {"IdEnsaio", ensaio.Id }
                        });
                        _context.SaveChanges();
                        await transaction.CommitAsync();

                        return HttpStatusCode.OK;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return HttpStatusCode.BadRequest;
                    }

                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpStatusCode.PreconditionFailed;
                }

            }
            catch
            {
                await transaction.RollbackAsync();
                return HttpStatusCode.InternalServerError;
            }
        }
        /// <summary>
        /// Deleta um Ensaio do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se deletou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public HttpStatusCode Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Ensaio? ensaio = _context.Ensaios.Where(es => es.Id == id)
                    .Select(es => new Ensaio()
                    {
                        Id = es.Id,
                        IdGrupoMusical = es.IdGrupoMusical,
                        IdColaboradorResponsavel = es.IdColaboradorResponsavel,
                        IdFigurinos = es.IdFigurinos,
                        Ensaiopessoas = es.Ensaiopessoas,
                    })
                    .FirstOrDefault();

                if (ensaio == null)
                {
                    transaction.Rollback();
                    return HttpStatusCode.NotFound;
                }

                if (ensaio.IdFigurinos.Count() > 0)
                {
                    foreach (var figurino in ensaio.IdFigurinos)
                    {
                        _context.Set<Dictionary<string, object>>("Figurinoensaio").Remove(new Dictionary<string, object>
                        {
                            { "IdFigurino", figurino.Id },
                            { "IdEnsaio", ensaio.Id }
                        });
                        _context.SaveChanges();
                    }
                }

                if (ensaio.Ensaiopessoas.Count() > 0)
                {
                    foreach (Ensaiopessoa p in ensaio.Ensaiopessoas)
                    {
                        _context.Remove(p);
                        _context.SaveChanges();
                    }
                }

                ensaio.IdFigurinos.Clear();
                ensaio.Ensaiopessoas.Clear();

                _context.Remove(ensaio);
                _context.SaveChanges();
                transaction.Commit();
                return HttpStatusCode.OK;

            }
            catch
            {
                transaction.Rollback();
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Edita um Ensaio do banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>retorna um inteiro.</returns>
        public async Task<HttpStatusCode> Edit(Ensaio ensaio, IEnumerable<int> idRegentes)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var ensaioDb = await _context.Ensaios.Where(e => e.Id == ensaio.Id).AsNoTracking().SingleOrDefaultAsync();
                if (ensaioDb == null)
                {
                    await transaction.RollbackAsync();
                    return HttpStatusCode.NotFound;
                }

                ensaio.IdColaboradorResponsavel = ensaioDb.IdColaboradorResponsavel;
                ensaio.IdGrupoMusical = ensaioDb.IdGrupoMusical;

                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if (ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        var idEnsaioRegentes = _context.Ensaiopessoas
                                            .Where(ep => ep.IdEnsaio == ensaioDb.Id)
                                            .Select(ep => ep.IdPessoa).AsEnumerable();

                        if ((idRegentes.Count() != idEnsaioRegentes.Count()) || (idRegentes.Except(idEnsaioRegentes).Any()))
                        {
                            var ensaioPessoaRegente = _context.Ensaiopessoas
                                                      .Where(ep => ep.IdEnsaio == ensaioDb.Id);
                            _context.Ensaiopessoas.RemoveRange(ensaioPessoaRegente);
                            foreach (int idRegente in idRegentes)
                            {
                                Ensaiopessoa ensaioPessoa = new()
                                {
                                    IdEnsaio = ensaio.Id,
                                    IdPessoa = idRegente,
                                    Presente = 1,
                                };
                                await _context.Ensaiopessoas.AddAsync(ensaioPessoa);
                            }
                        }
                        _context.Ensaios.Update(ensaio);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return HttpStatusCode.OK;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpStatusCode.PreconditionFailed;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return HttpStatusCode.InternalServerError;
            }
        }
        /// <summary>
        /// Consulta um Ensaio no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Ensaio correspondente ao id ou um Ensaio vazio</returns>
        public Ensaio Get(int id)
        {
            return _context.Ensaios.Find(id);
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

        public async Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO(int idGrupo)
        {
            var query = _context.Ensaios
                .OrderBy(g => g.DataHoraInicio)
                .Where(g => g.IdGrupoMusical == idGrupo)
                .Select(g => new EnsaioIndexDTO
                {
                    Id = g.Id,
                    DataHora = g.DataHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                    Tipo = g.Tipo,
                    Local = g.Local,
                    PresencaObrigatoria = g.PresencaObrigatoria == 1 ? "Sim" : "Não",
                    DataHoraInicio = g.DataHoraInicio

                }).AsNoTracking().ToListAsync();
            return await query;
        }

        public EnsaioDetailsDTO GetDetailsDTO(int idEnsaio)
        {
            var query = _context.Ensaios
                .Select(g => new EnsaioDetailsDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    DataHoraFim = g.DataHoraFim,
                    Tipo = g.Tipo,
                    Local = g.Local,
                    PresencaObrigatoria = g.PresencaObrigatoria == 1 ? "Sim" : "Não",
                    Repertorio = g.Repertorio,
                    Regentes = _context.Ensaiopessoas
                                       .Where(ep => ep.IdEnsaio == idEnsaio)
                                       .OrderBy(ep => ep.IdPessoaNavigation.Nome)
                                       .Select(ep => ep.IdPessoaNavigation.Nome).AsEnumerable(),
                    IdGrupoMusical = g.IdGrupoMusical

                }).Where(g => g.Id == idEnsaio);

            return query.First();
        }

        public HttpStatusCode RegistrarFrequencia(FrequenciaEnsaioDTO frequencia, int quantidadeAssociados)
        {
            try
            {              
                for (int i = 0; i < quantidadeAssociados; i++)
                {
                    var ensaioPessoa = new Ensaiopessoa
                    {
                        Presente = frequencia.AssociadosDTO[i].Presente,
                        JustificativaAceita = frequencia.AssociadosDTO[i].JustificativaAceita,
                        IdPessoa = frequencia.AssociadosDTO[i].Id,
                        IdEnsaio = frequencia.Id,
                        IdPapelGrupo = frequencia.AssociadosDTO[i].IdPapelGrupo
                    };
                    _context.Update(ensaioPessoa);
                }

                _context.SaveChanges();

                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }


        public IEnumerable<EnsaioAssociadoDTO>? GetEnsaiosEventosByIdPessoa(int idPessoa)
        {
            var query = from ensaioPessoa in _context.Ensaiopessoas
                        where ensaioPessoa.IdPessoa == idPessoa
                        select new EnsaioAssociadoDTO
                        {
                            Id = ensaioPessoa.IdEnsaio,
                            Inicio = ensaioPessoa.IdEnsaioNavigation.DataHoraInicio,
                            Fim = ensaioPessoa.IdEnsaioNavigation.DataHoraFim,
                            Presente = Convert.ToBoolean(ensaioPessoa.Presente),
                            Justificativa = ensaioPessoa.JustificativaFalta,
                            JustificativaAceita = Convert.ToBoolean(ensaioPessoa.JustificativaAceita),
                            Local = ensaioPessoa.IdEnsaioNavigation.Local,
                            Repertorio = ensaioPessoa.IdEnsaioNavigation.Repertorio
                        };

            return query.AsNoTracking().ToList();
        }

        public async Task<Ensaiopessoa?> GetEnsaioPessoaAsync(int idEnsaio, int idPessoa)
        {
            return await _context.Ensaiopessoas.Where(ep => ep.IdEnsaio == idEnsaio && ep.IdPessoa == idPessoa).FirstOrDefaultAsync();
        }

        public async Task<HttpStatusCode> RegistrarJustificativaAsync(int idEnsaio, int idPessoa, string? justificativa)
        {
            try
            {
                var ensaioPessoa = await GetEnsaioPessoaAsync(idEnsaio, idPessoa);
                if (ensaioPessoa == null)
                {
                    return HttpStatusCode.NotFound;
                }

                ensaioPessoa.JustificativaFalta = justificativa;
                ensaioPessoa.Presente = 0;

                _context.Update(ensaioPessoa);
                await _context.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<IEnumerable<int>> GetIdRegentesEnsaioAsync(int idEnsaio)
        {
            return await _context.Ensaiopessoas
                                 .Where(ep => ep.IdEnsaio == idEnsaio)
                                 .Select(ep => ep.IdPessoa).ToListAsync();
        }

        public List<AssociadoDTO> GetAssociadoAtivos(int idEnsaio)
        {
            var query = _context.Ensaiopessoas
                .Where(p => p.IdEnsaio == idEnsaio && p.IdPessoaNavigation.Ativo == 1 && p.IdPapelGrupo == 1)
                .Select(p => new AssociadoDTO { Id = p.IdPessoaNavigation.Id, Nome = p.IdPessoaNavigation.Nome, Cpf = p.IdPessoaNavigation.Cpf, IdPapelGrupo = p.IdPapelGrupo, JustificativaFalta = p.JustificativaFalta, Presente = p.Presente, JustificativaAceita = p.JustificativaAceita }).AsNoTracking().ToList();

            return query;
        }

        public async Task<DatatableResponse<EnsaioIndexDTO>> GetDataPage(DatatableRequest request, int idGrupo)
        {
            var ensaios = await GetAllIndexDTO(idGrupo);
            var totalRecords = ensaios.Count();

            // filtra pelo campos de busca
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                ensaios = ensaios.Where(ensaios => ensaios.Id.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                              || ensaios.Local.ToLower().Contains(request.Search.GetValueOrDefault("value")));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    ensaios = ensaios.OrderBy(ensaios => ensaios.DataHoraInicio);
                else
                    ensaios = ensaios.OrderByDescending(ensaios => ensaios.DataHoraInicio);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    ensaios = ensaios.OrderBy(ensaios => ensaios.Local);
                else
                    ensaios = ensaios.OrderByDescending(ensaios => ensaios.Local);
            }
            int countRecordsFiltered = ensaios.Count();

            ensaios = ensaios.Skip(request.Start).Take(request.Length);
            return new DatatableResponse<EnsaioIndexDTO>
            {
                Data = ensaios.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }

        public async Task<HttpStatusCode> NotificarEnsaioViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEnsaio)
        {
            try
            {
                var ensaio = Get(idEnsaio);
                if (ensaio != null)
                {
                    List<EmailModel> emailsBody = new List<EmailModel>();
                    foreach (PessoaEnviarEmailDTO p in pessoas)
                    {
                        emailsBody.Add(new EmailModel()
                        {
                            Assunto = "Batalá - Notificação de Ensaio",
                            AddresseeName = "Associados e Regentes",
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                    $"<h3>Notificação de um ensaio.</h3>\r\n" +
                                    "<div style=\"font-size: large;\">\r\n        " +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Início:</dt><dd>{ensaio.DataHoraInicio}</dd>" +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Fim:</dt><dd>{ensaio.DataHoraFim}</dd>\n</div>",
                            To = new List<string> { p.Email }
                        });
                    }

                    List<Task> emailTask = new List<Task>();
                    foreach (EmailModel ema in emailsBody)
                    {
                        emailTask.Add(EmailService.Enviar(ema));
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
