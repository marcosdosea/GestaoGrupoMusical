using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Core.Service.IEventoService;

namespace Service
{
    public class EventoService : IEventoService
    {
        private readonly GrupoMusicalContext _context;

        public EventoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método usado a para adicionar uma nova apresentação
        /// </summary>
        /// <param name="evento"></param>
        /// <returns>Id do Grupo Musical</returns>
        /// 
        public async Task<HttpStatusCode> Create(Evento evento, IEnumerable<int> idRegentes, int idFigurino)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (evento.DataHoraFim > evento.DataHoraInicio)
                {
                    if (evento.DataHoraInicio.Date >= DateTime.Today)
                    {
                        _context.Eventos.Add(evento);
                        _context.SaveChanges();
                        List<Eventopessoa> p = new();
                        foreach (int id in idRegentes)
                        {
                            p.Add(new Eventopessoa
                            {
                                IdEvento = evento.Id,
                                IdPessoa = id,
                                IdTipoInstrumento = 0,//por Default, o primeiro instrumento tem que ser o "nenhum". Foi gambiarra de dosea
                                IdPapelGrupoPapelGrupo = 5 //5 = Regente
                            });
                        }
                        _context.Eventopessoas.AddRange(p);
                        _context.SaveChanges();
                        _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Add(new Dictionary<string, object>
                        {
                            { "IdFigurino", idFigurino },
                            { "IdApresentacao", evento.Id }
                        });
                        _context.SaveChanges();
                        transaction.Commit();
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
        /// Método que deleta uma apresentação 
        /// </summary>
        /// <param name="id"></param>
        public HttpStatusCode Delete(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Evento? evento = _context.Eventos.Where(ev => ev.Id == id)
                    .Select(ev => new Evento()
                    {
                        Id = ev.Id,
                        IdGrupoMusical = ev.IdGrupoMusical,
                        IdColaboradorResponsavel = ev.IdColaboradorResponsavel,
                        IdFigurinos = ev.IdFigurinos,
                        Eventopessoas = ev.Eventopessoas,
                    })
                    .FirstOrDefault();
                if (evento == null)
                {
                    transaction.Rollback();
                    return HttpStatusCode.NotFound;
                }
                evento.Apresentacaotipoinstrumentos = _context.Apresentacaotipoinstrumentos.Where(ap => ap.IdApresentacao == evento.Id)
                    .Select(q => new Apresentacaotipoinstrumento()
                    {
                        IdApresentacao = q.IdApresentacao,
                        IdTipoInstrumento = q.IdTipoInstrumento,
                    }).ToList();

                if (evento.IdFigurinos.Count() > 0)
                {
                    foreach (var figurino in evento.IdFigurinos)
                    {
                        _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Remove(new Dictionary<string, object>
                        {
                            { "IdFigurino", figurino.Id },
                            { "IdApresentacao", evento.Id }
                        });
                        _context.SaveChanges();
                    }
                }

                if (evento.Apresentacaotipoinstrumentos.Count > 0)
                {
                    foreach (Apresentacaotipoinstrumento ap in evento.Apresentacaotipoinstrumentos)
                    {
                        _context.Remove(ap);
                        _context.SaveChanges();
                    }
                }

                if (evento.Eventopessoas.Count > 0)
                {
                    foreach (Eventopessoa p in evento.Eventopessoas)
                    {
                        _context.Remove(p);
                        _context.SaveChanges();
                    }
                }
                evento.IdFigurinos.Clear();
                evento.Apresentacaotipoinstrumentos.Clear();
                evento.Eventopessoas.Clear();

                _context.Remove(evento);
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
        /// Metodo usado para editar um Grupo Musical
        /// </summary>
        /// <param name="evento"></param>
        public HttpStatusCode Edit(Evento evento)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (evento.DataHoraFim > evento.DataHoraInicio)
                {
                    if (evento.DataHoraInicio.Date >= DateTime.Today)
                    {
                        //Caso existe já regentes cadastrado, primeiro deleta eles
                        var evPessoaRegentes = GetRegentesEventoPessoasPorIdEvento(evento.Id);
                        if (evPessoaRegentes.Count != 0)
                        {
                            _context.Eventopessoas.RemoveRange(evPessoaRegentes);
                            _context.SaveChanges();
                        }
                        //Caso existe já figurinos cadastrado, primeiro deleta eles
                        var figurinoApresentacoes = _context.Set<Dictionary<string, object>>("Figurinoapresentacao")
                        .Where(fa => (int)fa["IdApresentacao"] == evento.Id).ToList();
                        _context.Set<Dictionary<string, object>>("Figurinoapresentacao").RemoveRange(figurinoApresentacoes);
                        _context.SaveChanges();
                        figurinoApresentacoes = null;
                        //Adiciona os novos regentes
                        _context.AddRange(evento.Eventopessoas);
                        _context.SaveChanges();
                        //Adicinoa um figurino
                        _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Add(new Dictionary<string, object>
                                      {
                                          { "IdFigurino", evento.IdFigurinos.First().Id },
                                          { "IdApresentacao", evento.Id }
                                      });
                        _context.SaveChanges();
                        evento.IdFigurinos.Clear();
                        evento.Eventopessoas.Clear();
                        evento.Apresentacaotipoinstrumentos.Clear();
                        _context.Update(evento);
                        _context.SaveChanges();
                        transaction.Commit();
                        return HttpStatusCode.OK;
                    }
                    else
                    {
                        transaction.Rollback();
                        return HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    transaction.Rollback();
                    return HttpStatusCode.PreconditionFailed;
                }
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public Evento? Get(int id)
        {
            return _context.Eventos.Find(id);
        }

        public ICollection<Eventopessoa> GetEventoPessoasPorIdEvento(int idEvento)
        {
            var query = (from eventoPessoa in _context.Eventopessoas
                         where eventoPessoa.IdEvento == idEvento
                         select eventoPessoa).AsNoTracking().ToList();
            return query;
        }

        public ICollection<Eventopessoa> GetRegentesEventoPessoasPorIdEvento(int idEvento)
        {
            var query = (from eventoPessoa in _context.Eventopessoas
                         where eventoPessoa.IdEvento == idEvento && eventoPessoa.IdPapelGrupoPapelGrupo == 5
                         select eventoPessoa).AsNoTracking().ToList();
            return query;
        }

        public IEnumerable<Evento> GetAll()
        {
            return _context.Eventos.AsNoTracking();
        }


        public IEnumerable<EventoDTO> GetAllDTO()
        {
            var query = _context.Eventos
                .OrderBy(g => g.DataHoraInicio)
                .Select(g =>
                new EventoDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local
                });

            return query.AsNoTracking();
        }

        public IEnumerable<EventoIndexDTO> GetAllIndexDTO()
        {
            var query = _context.Eventos
                .OrderBy(g => g.DataHoraInicio).
                Select(g => new EventoIndexDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local,
                    Planejados = 0,
                    Confirmados = 0
                }
                ).AsNoTracking();
            return query;
        }

        public IEnumerable<EventoIndexDTO> GetAllEventoIndexDTOPerIdGrupoMusical(int idGrupoMusical)
        {
            var query = _context.Eventos.Where(g => g.IdGrupoMusical == idGrupoMusical)
                .OrderBy(g => g.DataHoraInicio).
                Select(g => new EventoIndexDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local,
                    Planejados = 0,
                    Confirmados = 0,
                }
                ).AsNoTracking();
            return query;
        }

        public DatatableResponse<EventoIndexDTO> GetDataPage(DatatableRequest request, int idGrupo)
        {
            var eventos = GetAllEventoIndexDTOPerIdGrupoMusical(idGrupo);

            var totalRecords = eventos.Count();

            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                eventos = eventos.Where(g => g.DataHoraInicio.ToString().Contains(request.Search.GetValueOrDefault("value")!)
                                                           || g.Local.ToString().Contains(request.Search.GetValueOrDefault("value")!));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    eventos = eventos.OrderBy(g => g.DataHoraInicio);
                else
                    eventos = eventos.OrderByDescending(g => g.DataHoraInicio);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    eventos = eventos.OrderBy(g => g.Local);
                else
                    eventos = eventos.OrderByDescending(g => g.Local);
            }

            int countRecordsFiltered = eventos.Count();

            eventos = eventos.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<EventoIndexDTO>
            {
                Data = eventos.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }

        public HttpStatusCode NotificarEventoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEvento)
        {
            try
            {
                var evento = Get(idEvento);
                if (evento != null)
                {
                    List<EmailModel> emailsBody = new List<EmailModel>();
                    foreach (PessoaEnviarEmailDTO p in pessoas)
                    {
                        emailsBody.Add(new EmailModel()
                        {
                            Assunto = $"Batalá - Notificação de Evento: {evento.Repertorio}",
                            AddresseeName = p.Nome,
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h3>O evento foi aprovado!</h3>\r\n</div>",
                            To = new List<string> { p.Email }

                        });
                    }

                    List<Task> emailTask = new List<Task>();
                    foreach (EmailModel email in emailsBody)
                    {
                        emailTask.Add(EmailService.Enviar(email));
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
        public async Task<string> GetNomeInstrumento(int id)
        {
            var query = await (from instrumento in _context.Instrumentomusicals
                               where instrumento.Id == id
                               select new { instrumento.IdTipoInstrumentoNavigation.Nome }).AsNoTracking().SingleOrDefaultAsync();

            return query?.Nome ?? "";
        }
        public async Task<IEnumerable<FigurinoDropdownDTO>> GetAllFigurinoDropdown(int idGrupo)
        {
            var query = await _context.Figurinos.Where(p => p.IdGrupoMusical == idGrupo)
                .Select(g => new FigurinoDropdownDTO
                {
                    Id = g.Id,
                    Nome = g.Nome
                }).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<IEnumerable<Eventopessoa>> GetPessoas(int idGrupo)
        {
            var query = await _context.Eventopessoas.Where(p => p.IdEvento == idGrupo)
               .Select(g => new Eventopessoa
               {
                   IdEvento = g.IdEvento,
                   IdPessoa = g.IdPessoa
               }).AsNoTracking().ToListAsync();

            return query;
        }

        public async Task<HttpStatusCode> CreateApresentacaoInstrumento(Apresentacaotipoinstrumento apresentacaotipoinstrumento)
        {
            await _context.Apresentacaotipoinstrumentos.AddAsync(apresentacaotipoinstrumento);
            await _context.SaveChangesAsync();

            return HttpStatusCode.Created;
        }

        public IEnumerable<SolicitacaoEventoPessoasDTO> GetSolicitacaoEventoPessoas(int idEvento)
        {
            DateTime twoMonthsAgo = DateTime.Now.AddMonths(-2);
            var query = (from evento in _context.Eventos
                         join eventoPessoa in _context.Eventopessoas
                         on evento.Id equals eventoPessoa.IdEvento
                         join tipoInstrumento in _context.Tipoinstrumentos
                         on eventoPessoa.IdTipoInstrumento equals tipoInstrumento.Id
                         join pessoa in _context.Pessoas
                         on eventoPessoa.IdPessoa equals pessoa.Id
                         where idEvento == evento.Id
                         select new SolicitacaoEventoPessoasDTO
                         {
                             IdInstrumento = tipoInstrumento.Id,
                             NomeInstrumento = tipoInstrumento.Nome,
                             IdAssociado = eventoPessoa.IdPessoa,
                             IdPapelGrupo = eventoPessoa.IdPapelGrupoPapelGrupo,
                             NomeAssociado = pessoa.Nome,
                             Faltas = _context.Ensaiopessoas.
                             Count(
                                 ep => ep.IdPessoa == pessoa.Id &&
                                 ep.Presente == 0 &&
                                 ep.JustificativaAceita == 0 &&
                                 ep.IdEnsaioNavigation.DataHoraInicio >= twoMonthsAgo &&
                                 ep.IdEnsaioNavigation.PresencaObrigatoria == 1),
                             //antes de pegar inadinplencia, pergunta se a pessoa eh um associado,
                             //pois apenas sao pros associados evitando fazer consultas descenessarias.
                             Inadiplencia = eventoPessoa.IdPapelGrupoPapelGrupo == 1 ?
                             _context.Receitafinanceiras.Where
                             (
                                 rf => rf.Receitafinanceirapessoas.Any(
                                     rfp => rfp.IdPessoa == eventoPessoa.IdPessoa &&
                                       rfp.Status != "PAGO" &&
                                       rfp.Status != "ISENTO" &&
                                     rf.DataFim < DateTime.Now.Date
                                     )).Count() : 0,
                             AprovadoModel = ConvertAprovadoParaEnum(eventoPessoa.Status),
                             Aprovado = ConvertAprovadoParaEnum(eventoPessoa.Status)
                         }).AsNoTracking().ToList();
            return query;
        }



        public GerenciarSolicitacaoEventoDTO? GetSolicitacoesEventoDTO(int idEvento)
        {
            Evento? evento = Get(idEvento);
            if (evento == null)
                return null;

            GerenciarSolicitacaoEventoDTO g = new()
            {
                Id = idEvento,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
            };
            g.EventoSolicitacaoPessoasDTO = GetSolicitacaoEventoPessoas(idEvento);
            foreach (SolicitacaoEventoPessoasDTO s in g.EventoSolicitacaoPessoasDTO)
            {
                if (s.IdPapelGrupo == 5)
                {
                    if (g.NomesRegentes.Length > 0)
                        g.NomesRegentes += "; " + s.NomeAssociado;
                    else
                        g.NomesRegentes = s.NomeAssociado;
                }
            }
            g.EventoSolicitacaoPessoasDTO = g.EventoSolicitacaoPessoasDTO.Where(e => e.IdPapelGrupo != 5);
            return g;
        }

        public EventoStatus EditSolicitacoesEvento(GerenciarSolicitacaoEventoDTO g)
        {
            //using var transaction = _context.Database.BeginTransaction();
            Console.WriteLine("\n##### SERVICE #####");
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (g.EventoSolicitacaoPessoasDTO == null || !g.EventoSolicitacaoPessoasDTO.Any())
                    return EventoStatus.ErroGenerico;
                //Remover os que NAO foram editados para aumentar o desempenho.
                g.EventoSolicitacaoPessoasDTO = g.EventoSolicitacaoPessoasDTO.Where(e => e.Aprovado != e.AprovadoModel);
                if (g.EventoSolicitacaoPessoasDTO.Count() == 0)
                {
                    transaction.Rollback();
                    return EventoStatus.SemAlteracao;
                }

                //primeiro verifica se houve mudancas de INSCRITO para INDEFERIDO ou mudancas que
                //foram de INDEFERIDO para INSCRITO, pois nao ha impacto em outra tabela
                List<SolicitacaoEventoPessoasDTO> auxSolicitacaoEvento = g.EventoSolicitacaoPessoasDTO.Where(
                    es => (es.Aprovado == InscricaoEventoPessoa.INSCRITO && es.AprovadoModel == InscricaoEventoPessoa.INDEFERIDO) ||
                    (es.Aprovado == InscricaoEventoPessoa.INDEFERIDO && es.AprovadoModel == InscricaoEventoPessoa.INSCRITO)
                    ).ToList();
                if (auxSolicitacaoEvento.Count != 0)
                {
                    for (int i = 0; i < auxSolicitacaoEvento.Count; i++)
                    {
                        Eventopessoa? e = _context.Eventopessoas.Where(
                            ep => ep.IdPessoa == auxSolicitacaoEvento[i].IdAssociado &&
                            ep.IdTipoInstrumento == auxSolicitacaoEvento[i].IdInstrumento &&
                            ep.IdEvento == g.Id
                        ).FirstOrDefault();
                        if (e != null)
                        {
                            e.Status = auxSolicitacaoEvento[i].AprovadoModel.ToString();
                            _context.Update(e);
                            _context.SaveChanges();
                        }
                    }
                }
                //Agora filtro para mexer na tabela ApresentacaoTipoInstrumento
                g.EventoSolicitacaoPessoasDTO = g.EventoSolicitacaoPessoasDTO.Where(
                    e => e.Aprovado == InscricaoEventoPessoa.DEFERIDO ||
                    e.AprovadoModel == InscricaoEventoPessoa.DEFERIDO
                    );
                Console.WriteLine("#### APRESENTACAOTIPOINSTRUMENTO ####");
                Console.WriteLine("Sobraram: " + g.EventoSolicitacaoPessoasDTO.Count());
                if (g.EventoSolicitacaoPessoasDTO.Count() > 0)
                {
                    List<Apresentacaotipoinstrumento> at = _context.Apresentacaotipoinstrumentos.Where(
                        ati => ati.IdApresentacao == g.Id &&
                        g.EventoSolicitacaoPessoasDTO.Select(ev => ev.IdInstrumento).ToList().Contains(ati.IdTipoInstrumento)
                        ).AsNoTracking().ToList();
                    Console.WriteLine("#### APRESENTACAOTIPOINSTRUMENTO ####");
                    Console.WriteLine("APCount: " + at.Count());
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return EventoStatus.ErroGenerico;
            }

            return EventoStatus.Success;
        }
        public async Task<EventoFrequenciaDTO?> GetFrequenciaAsync(int idEvento, int idGrupoMusical)
        {
            var regentes = await _context.Eventopessoas
                .Where(ep => ep.IdEvento == idEvento)
                .OrderBy(ep => ep.IdPessoaNavigation.Nome)
                .Select(ep => ep.IdPessoaNavigation.Nome)
                .ToListAsync();

            var frequencias = await _context.Eventopessoas
                .Where(eventoPessoa => eventoPessoa.IdEvento == idEvento)
                .OrderBy(eventoPessoa => eventoPessoa.IdPessoaNavigation.Nome)
                .Select(eventoPessoa => new EventoListaFrequenciaDTO
                {
                    IdEvento = eventoPessoa.IdEvento,
                    IdPessoa = eventoPessoa.IdPessoa,
                    Cpf = eventoPessoa.IdPessoaNavigation.Cpf,
                    NomeAssociado = eventoPessoa.IdPessoaNavigation.Nome,
                    Justificativa = eventoPessoa.JustificativaFalta,
                    Presente = Convert.ToBoolean(eventoPessoa.Presente),
                    JustificativaAceita = Convert.ToBoolean(eventoPessoa.JustificativaAceita),
                }).ToListAsync();

            var query = from evento in _context.Eventos
                        where evento.Id == idEvento && evento.IdGrupoMusical == idGrupoMusical
                        select new EventoFrequenciaDTO
                        {
                            Inicio = evento.DataHoraInicio,
                            Fim = evento.DataHoraFim,
                            Regentes = regentes,
                            Local = evento.Local,
                            Frequencias = frequencias
                        };

            return await query.AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task<HttpStatusCode> RegistrarFrequenciaAsync(List<EventoListaFrequenciaDTO> frequencias)
        {
            try
            {
                if (!frequencias.Any())
                {
                    return HttpStatusCode.BadRequest;
                }
                int idEvento = frequencias.First().IdEvento;

                var dbFrequencias = _context.Eventopessoas
                                    .Where(ep => ep.IdEvento == frequencias.First().IdEvento)
                                    .OrderBy(ep => ep.IdPessoaNavigation.Nome);

                if (dbFrequencias == null)
                {
                    return HttpStatusCode.NotFound;
                }

                if (dbFrequencias.Count() != frequencias.Count)
                {
                    return HttpStatusCode.Conflict;
                }

                int pos = 0;
                await dbFrequencias.ForEachAsync(dbFrequencia =>
                {
                    if (dbFrequencia.IdEvento == frequencias[0].IdEvento && dbFrequencia.IdPessoa == frequencias[pos].IdPessoa)
                    {
                        dbFrequencia.JustificativaAceita = Convert.ToSByte(frequencias[pos].JustificativaAceita);
                        dbFrequencia.Presente = Convert.ToSByte(frequencias[pos].Presente);

                        _context.Update(dbFrequencia);
                    }
                    pos++;
                });

                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<IEnumerable<EventoAssociadoDTO>> GetEventosByIdPessoaAsync(int idPessoa)
        {
            var query = from eventoPessoa in _context.Eventopessoas
                        where eventoPessoa.IdPessoa == idPessoa
                        select new EventoAssociadoDTO
                        {
                            IdEvento = eventoPessoa.IdEvento,
                            Inicio = eventoPessoa.IdEventoNavigation.DataHoraInicio,
                            Fim = eventoPessoa.IdEventoNavigation.DataHoraFim,
                            Presente = Convert.ToBoolean(eventoPessoa.Presente),
                            Justificativa = eventoPessoa.JustificativaFalta,
                            JustificativaAceita = Convert.ToBoolean(eventoPessoa.JustificativaAceita),
                            Local = eventoPessoa.IdEventoNavigation.Local,
                            Repertorio = eventoPessoa.IdEventoNavigation.Repertorio
                        };

            return await query.AsNoTracking().ToListAsync();
        }
        public async Task<Eventopessoa?> GetEventoPessoaAsync(int idEvento, int idPessoa)
        {
            return await _context.Eventopessoas.Where(ep => ep.IdEvento == idEvento && ep.IdPessoa == idPessoa).FirstOrDefaultAsync();
        }
        public async Task<HttpStatusCode> RegistrarJustificativaAsync(int idEvento, int idPessoa, string? justificativa)
        {
            try
            {
                var eventoPessoa = await GetEventoPessoaAsync(idEvento, idPessoa);
                if (eventoPessoa == null)
                {
                    return HttpStatusCode.NotFound;
                }

                eventoPessoa.JustificativaFalta = justificativa;
                eventoPessoa.Presente = 0;

                _context.Update(eventoPessoa);
                await _context.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }
        public async Task<IEnumerable<int>> GetIdRegentesEventoAsync(int idEnsaio)
        {
            return await _context.Ensaiopessoas
                                 .Where(ep => ep.IdEnsaio == idEnsaio)
                                 .Select(ep => ep.IdPessoa).ToListAsync();
        }

    }
}
