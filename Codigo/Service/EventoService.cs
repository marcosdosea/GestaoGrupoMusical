using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Crypto;
using System.Net;
using static Core.Service.IEventoService;


namespace Service
{
    public class EventoService : IEventoService
    {
        private readonly GrupoMusicalContext _context;
        private readonly IMapper _mapper;


        public EventoService(GrupoMusicalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

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
            var query = _context.Eventos.Where(g => g.IdGrupoMusical == idGrupoMusical).
                Select(g => new EventoIndexDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local,
                    Planejados = _context.Apresentacaotipoinstrumentos.
                    Where(ap => ap.IdApresentacao == g.Id).Sum(ap => ap.QuantidadePlanejada),
                    Confirmados = _context.Apresentacaotipoinstrumentos.
                    Where(ap => ap.IdApresentacao == g.Id).Sum(ap => ap.QuantidadeConfirmada)
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
                if (!request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
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
            try
            {                
                bool exists = await _context.Apresentacaotipoinstrumentos
                    .AnyAsync(a => a.IdTipoInstrumento == apresentacaotipoinstrumento.IdTipoInstrumento && a.IdApresentacao == apresentacaotipoinstrumento.IdApresentacao);

                if (exists)
                {
                    
                    return HttpStatusCode.Conflict;
                }
                
                await _context.Apresentacaotipoinstrumentos.AddAsync(apresentacaotipoinstrumento);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            }         
            catch (Exception ex)
            {
                               
                return HttpStatusCode.InternalServerError; 
            }
        }

        public IEnumerable<SolicitacaoEventoPessoasDTO> GetSolicitacaoEventoPessoas(int idEvento, int pegarFaltasEmMesesAtras)
        {
            DateTime doisMesesAtras = DateTime.Now.AddMonths(pegarFaltasEmMesesAtras);
            var query = (from eventoPessoa in _context.Eventopessoas
                         where idEvento == eventoPessoa.IdEvento
                         select new SolicitacaoEventoPessoasDTO
                         {
                             IdInstrumento = eventoPessoa.IdTipoInstrumentoNavigation.Id,
                             NomeInstrumento = eventoPessoa.IdTipoInstrumentoNavigation.Nome,
                             IdAssociado = eventoPessoa.IdPessoa,
                             IdPapelGrupo = eventoPessoa.IdPapelGrupoPapelGrupo,
                             NomeAssociado = eventoPessoa.IdPessoaNavigation.Nome,
                             AprovadoModel = ConvertAprovadoParaEnum(eventoPessoa.Status),
                             Aprovado = ConvertAprovadoParaEnum(eventoPessoa.Status)
                         }).AsNoTracking().ToList();

            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].IdPapelGrupo != 1)
                    continue;
                query[i].Faltas = _context.Ensaiopessoas.AsNoTracking().
                             Count(
                                 ep => ep.IdPessoa == query[i].IdAssociado &&
                                 ep.Presente == 0 &&
                                 ep.JustificativaAceita == 0 &&
                                 ep.IdEnsaioNavigation.DataHoraInicio >= doisMesesAtras &&
                                 ep.IdEnsaioNavigation.PresencaObrigatoria == 1);

                query[i].Inadiplencia = _context.Receitafinanceiras.Where
                    (
                    rf => rf.Receitafinanceirapessoas.Any(
                        rfp => rfp.IdPessoa == query[i].IdAssociado &&
                        rfp.Status != "PAGO" &&
                        rf.DataFim > DateTime.Now.Date
                        )).AsNoTracking().Count();
            }
            return query;
        }



        public GerenciarSolicitacaoEventoDTO? GetSolicitacoesEventoDTO(int idEvento, int pegarFaltasEmMesesAtras)
        {
            Evento? evento = Get(idEvento);
            if (evento == null)
                return null;

            GerenciarSolicitacaoEventoDTO g = new()
            {
                Id = idEvento,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                FaltasPessoasEmEnsaioMeses = pegarFaltasEmMesesAtras
            };
            g.EventoSolicitacaoPessoasDTO = GetSolicitacaoEventoPessoas(idEvento, pegarFaltasEmMesesAtras);
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
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (g.EventoSolicitacaoPessoasDTO == null || g.EventoSolicitacaoPessoasDTO.Count() < 1)
                {
                    transaction.Rollback();
                    return EventoStatus.SemAlteracao;
                }
                g.EventoSolicitacaoPessoasDTO = g.EventoSolicitacaoPessoasDTO.Where(e => e.Aprovado != e.AprovadoModel);
                if (g.EventoSolicitacaoPessoasDTO.Count() == 0)
                {
                    transaction.Rollback();
                    return EventoStatus.SemAlteracao;
                }
                List<int> auxIdInstrumento = g.EventoSolicitacaoPessoasDTO.Select(ev => ev.IdInstrumento).ToList();
                List<Apresentacaotipoinstrumento> at = _context.Apresentacaotipoinstrumentos.Where(
                       ati => ati.IdApresentacao == g.Id &&
                       auxIdInstrumento.Contains(ati.IdTipoInstrumento)
                       ).AsNoTracking().ToList();
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
                        else
                        {
                            transaction.Rollback();
                            return EventoStatus.ErroGenerico;
                        }
                    }
                }
                g.EventoSolicitacaoPessoasDTO = g.EventoSolicitacaoPessoasDTO.Where(
                    e => e.Aprovado == InscricaoEventoPessoa.DEFERIDO ||
                    e.AprovadoModel == InscricaoEventoPessoa.DEFERIDO
                    );
                if (g.EventoSolicitacaoPessoasDTO.Count() > 0)
                {
                    auxSolicitacaoEvento = g.EventoSolicitacaoPessoasDTO.Where(
                        ep => ep.Aprovado == InscricaoEventoPessoa.DEFERIDO || 
                        ep.AprovadoModel == InscricaoEventoPessoa.DEFERIDO
                        ).ToList();
                    if (auxSolicitacaoEvento.Count != 0)
                    {
                        for (int i = 0; i < auxSolicitacaoEvento.Count; i++)
                        {
                            Eventopessoa? e = _context.Eventopessoas.Where(
                            ep => ep.IdPessoa == auxSolicitacaoEvento[i].IdAssociado &&
                            ep.IdTipoInstrumento == auxSolicitacaoEvento[i].IdInstrumento &&
                            ep.IdEvento == g.Id).FirstOrDefault();
                            Apresentacaotipoinstrumento? auxAt = at.Where(
                                        at => at.IdApresentacao == g.Id
                                        && at.IdTipoInstrumento == auxSolicitacaoEvento[i].IdInstrumento).FirstOrDefault();
                            if (auxAt != null && e != null)
                            {
                                if (auxSolicitacaoEvento[i].Aprovado == InscricaoEventoPessoa.DEFERIDO)
                                {
                                    auxAt.QuantidadeConfirmada--;
                                }
                                else if(auxSolicitacaoEvento[i].Aprovado == InscricaoEventoPessoa.INDEFERIDO || auxSolicitacaoEvento[i].Aprovado == InscricaoEventoPessoa.INSCRITO)
                                {
                                    auxAt.QuantidadeConfirmada++;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return EventoStatus.ErroGenerico;
                                }
                                if(auxAt.QuantidadeConfirmada < 0)
                                {
                                    transaction.Rollback();
                                    return EventoStatus.QuantidadeConfirmadaNegativa;
                                }
                                if(auxAt.QuantidadeConfirmada > auxAt.QuantidadePlanejada)
                                {
                                    transaction.Rollback();
                                    return EventoStatus.UltrapassouLimiteQuantidadePlanejada;
                                }
                                
                                e.Status = auxSolicitacaoEvento[i].AprovadoModel.ToString();
                                _context.Update(e);
                                _context.SaveChanges();
                            }
                            else
                            {
                                transaction.Rollback();
                                return EventoStatus.ErroGenerico;
                            }
                        }
                    }
                }
                _context.UpdateRange(at);
                _context.SaveChanges();
                transaction.Commit();
                return EventoStatus.Success;
            }
            catch
            {
                transaction.Rollback();
                return EventoStatus.ErroGenerico;
            }
        }


    }
}