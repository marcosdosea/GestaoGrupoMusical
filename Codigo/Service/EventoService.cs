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
            var transaction = _context.Database.BeginTransaction();

            try
            {
                if (evento.DataHoraFim > evento.DataHoraInicio)
                {
                    if (evento.DataHoraInicio >= DateTime.Now)
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
                                IdPapelGrupoPapelGrupo = 5,
                                Status = "INSCRITO",
                                Presente = 0,
                                JustificativaAceita = 0
                            });
                        }
                        _context.Eventopessoas.AddRange(p);
                        _context.SaveChanges();
                        _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Add(new Dictionary<string, object>
                        {
                            {"IdFigurino", idFigurino },
                            {"IdApresentacao", evento.Id }
                        });
                        _context.SaveChanges();

                        IEnumerable<int> idAssociados = _context.Pessoas.Where
                            (p => p.IdGrupoMusical == evento.IdGrupoMusical && p.IdPapelGrupo == (int)PapelGrupo.ASSOCIADO).Select(p => p.Id);
                        if (idAssociados.Any())
                        {
                            List<Eventopessoa> ep = new();
                            foreach (int id in idAssociados)
                            {
                                ep.Add(new Eventopessoa()
                                {
                                    IdEvento = evento.Id,
                                    IdPessoa = id,
                                    IdPapelGrupoPapelGrupo = 5,
                                    Status = "INSCRITO",  
                                    Presente = 0,
                                    JustificativaAceita = 0
                                });
                            }
                            _context.AddRange(ep);
                            _context.SaveChanges();
                        }
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
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
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
        /// Metodo usado para editar um Evento
        /// </summary>
        /// <param name="evento">O objeto evento com as novas informações</param>
        /// <param name="idRegentes">Uma lista com os IDs dos novos regentes</param>
        /// <param name="idFigurino">O ID do novo figurino</param>
        public HttpStatusCode Edit(Evento evento, IEnumerable<int> idRegentes, int idFigurino)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (evento.DataHoraFim <= evento.DataHoraInicio)
                {
                    transaction.Rollback();
                    return HttpStatusCode.PreconditionFailed;
                }

                if (evento.DataHoraInicio.Date < DateTime.Today)
                {
                    transaction.Rollback();
                    return HttpStatusCode.BadRequest;
                }

                // 1. Remover regentes antigos
                var regentesAtuais = _context.Eventopessoas
                                        .Where(ep => ep.IdEvento == evento.Id && ep.IdPapelGrupoPapelGrupo == 5);
                _context.Eventopessoas.RemoveRange(regentesAtuais);
                _context.SaveChanges();

                // 2. Adicionar novos regentes
                foreach (var idRegente in idRegentes)
                {
                    var novoRegente = new Eventopessoa
                    {
                        IdEvento = evento.Id,
                        IdPessoa = idRegente,
                        IdPapelGrupoPapelGrupo = 5, // Papel de Regente
                        Status = "INSCRITO"
                    };
                    _context.Eventopessoas.Add(novoRegente);
                }
                _context.SaveChanges();

                // 3. Remover figurino antigo
                var figurinoAntigo = _context.Set<Dictionary<string, object>>("Figurinoapresentacao")
                                        .FirstOrDefault(fa => (int)fa["IdApresentacao"] == evento.Id);
                if (figurinoAntigo != null)
                {
                    _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Remove(figurinoAntigo);
                    _context.SaveChanges();
                }

                // 4. Adicionar novo figurino
                _context.Set<Dictionary<string, object>>("Figurinoapresentacao").Add(new Dictionary<string, object>
        {
            { "IdFigurino", idFigurino },
            { "IdApresentacao", evento.Id }
        });
                _context.SaveChanges();

                // 5. Limpar coleções para evitar problemas de tracking do EF
                evento.Eventopessoas.Clear();
                evento.IdFigurinos.Clear();

                // 6. Atualizar o evento
                _context.Eventos.Update(evento);
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

            var dadosBrutos = (from eventoPessoa in _context.Eventopessoas
                               where idEvento == eventoPessoa.IdEvento
                               select new
                               {
                                   IdInstrumento = eventoPessoa.IdTipoInstrumento ?? 0, 
                                   NomeInstrumento = eventoPessoa.IdTipoInstrumentoNavigation != null
                                                   ? eventoPessoa.IdTipoInstrumentoNavigation.Nome
                                                   : "Sem Instrumento", // Verificação de null
                                   IdAssociado = eventoPessoa.IdPessoa,
                                   IdPapelGrupo = eventoPessoa.IdPapelGrupoPapelGrupo,
                                   NomeAssociado = eventoPessoa.IdPessoaNavigation.Nome,
                                   Status = eventoPessoa.Status
                               }).AsNoTracking().ToList();

            var query = dadosBrutos.Select(item => new SolicitacaoEventoPessoasDTO
            {
                IdInstrumento = item.IdInstrumento,
                NomeInstrumento = item.NomeInstrumento,
                IdAssociado = item.IdAssociado,
                IdPapelGrupo = item.IdPapelGrupo,
                NomeAssociado = item.NomeAssociado,
                AprovadoModel = ConvertAprovadoParaEnum(item.Status),
                Aprovado = ConvertAprovadoParaEnum(item.Status)
            }).ToList();

            // Calcular faltas e inadimplência apenas para associados
            for (int i = 0; i < query.Count; i++)
            {
                if (query[i].IdPapelGrupo != 1) // Se não é associado, pula
                    continue;

                query[i].Faltas = _context.Ensaiopessoas.AsNoTracking()
                             .Count(ep => ep.IdPessoa == query[i].IdAssociado &&
                                    ep.Presente == 0 &&
                                    ep.JustificativaAceita == 0 &&
                                    ep.IdEnsaioNavigation.DataHoraInicio >= doisMesesAtras &&
                                    ep.IdEnsaioNavigation.PresencaObrigatoria == 1);

                query[i].Inadiplencia = _context.Receitafinanceiras
                    .Where(rf => rf.Receitafinanceirapessoas.Any(rfp =>
                        rfp.IdPessoa == query[i].IdAssociado &&
                        rfp.Status != "PAGO" &&
                        rf.DataFim > DateTime.Now.Date))
                    .AsNoTracking().Count();
            }

            return query;
        }

        public IEnumerable<InstrumentoSolicitacaoDTO> GetInstrumentosDisponiveis(int idEvento)
        {
            var query = from ati in _context.Apresentacaotipoinstrumentos
                        join ti in _context.Tipoinstrumentos on ati.IdTipoInstrumento equals ti.Id
                        where ati.IdApresentacao == idEvento
                        select new InstrumentoSolicitacaoDTO
                        {
                            IdInstrumento = ti.Id,
                            NomeInstrumento = ti.Nome,
                            QuantidadePlanejada = ati.QuantidadePlanejada,
                            QuantidadeConfirmada = ati.QuantidadeConfirmada,
                            QuantidadeSolicitada = ati.QuantidadeSolicitada,
                            VagasDisponiveis = ati.QuantidadePlanejada - ati.QuantidadeConfirmada
                        };

            return query.AsNoTracking().ToList();
        }

        public async Task<HttpStatusCode> SolicitarParticipacao(int idEvento, int idPessoa, int idTipoInstrumento)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                // Verificar se o associado já está inscrito no evento
                var jaInscrito = await _context.Eventopessoas
                    .AnyAsync(ep => ep.IdEvento == idEvento && ep.IdPessoa == idPessoa);

                if (jaInscrito)
                {
                    // Verificar se já solicitou este instrumento
                    var jaTemInstrumento = await _context.Eventopessoas
                        .AnyAsync(ep => ep.IdEvento == idEvento &&
                                       ep.IdPessoa == idPessoa &&
                                       ep.IdTipoInstrumento == idTipoInstrumento);

                    if (jaTemInstrumento)
                    {
                        transaction.Rollback();
                        return HttpStatusCode.Conflict; // Já solicitou este instrumento
                    }

                    // Atualizar o registro existente
                    var eventoExistente = await _context.Eventopessoas
                        .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdPessoa == idPessoa);

                    if (eventoExistente != null)
                    {
                        eventoExistente.IdTipoInstrumento = idTipoInstrumento;
                        eventoExistente.Status = InscricaoEventoPessoa.INSCRITO.ToString();
                        _context.Update(eventoExistente);
                    }
                }
                else
                {
                    // Criar nova solicitação
                    var novaInscricao = new Eventopessoa
                    {
                        IdEvento = idEvento,
                        IdPessoa = idPessoa,
                        IdTipoInstrumento = idTipoInstrumento,
                        IdPapelGrupoPapelGrupo = 1, 
                        Status = InscricaoEventoPessoa.INSCRITO.ToString(),
                        Presente = 0,
                        JustificativaAceita = 0
                    };

                    _context.Eventopessoas.Add(novaInscricao);
                }

                // Atualizar quantidade solicitada
                var instrumentoEvento = await _context.Apresentacaotipoinstrumentos
                    .FirstOrDefaultAsync(ati => ati.IdApresentacao == idEvento &&
                                               ati.IdTipoInstrumento == idTipoInstrumento);

                if (instrumentoEvento != null)
                {
                    instrumentoEvento.QuantidadeSolicitada++;
                    _context.Update(instrumentoEvento);
                }

                await _context.SaveChangesAsync();
                transaction.Commit();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> CancelarSolicitacao(int idEvento, int idPessoa)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var eventoPessoa = await _context.Eventopessoas
                    .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdPessoa == idPessoa);

                if (eventoPessoa == null)
                {
                    transaction.Rollback();
                    return HttpStatusCode.NotFound;
                }

                // Só pode cancelar se ainda não foi aprovado
                if (eventoPessoa.Status == InscricaoEventoPessoa.DEFERIDO.ToString())
                {
                    transaction.Rollback();
                    return HttpStatusCode.BadRequest; 
                }

                if (eventoPessoa.IdTipoInstrumento.HasValue)
                {
                    var instrumentoEvento = await _context.Apresentacaotipoinstrumentos
                        .FirstOrDefaultAsync(ati => ati.IdApresentacao == idEvento &&
                                                   ati.IdTipoInstrumento == eventoPessoa.IdTipoInstrumento);

                    if (instrumentoEvento != null)
                    {
                        instrumentoEvento.QuantidadeSolicitada--;
                        _context.Update(instrumentoEvento);
                    }
                }

                eventoPessoa.Status = InscricaoEventoPessoa.NAO_SOLICITADO.ToString();
                eventoPessoa.IdTipoInstrumento = null;
                _context.Update(eventoPessoa);

                await _context.SaveChangesAsync();
                transaction.Commit();
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<bool> PodeAssociadoSolicitar(int idEvento, int idPessoa)
        {
            var evento = await _context.Eventos.FindAsync(idEvento);
            if (evento == null || evento.DataHoraInicio <= DateTime.Now)
            {
                return false; // Evento não existe ou já passou
            }

            var pessoa = await _context.Pessoas.FindAsync(idPessoa);
            if (pessoa == null || pessoa.IdPapelGrupo != (int)PapelGrupo.ASSOCIADO)
            {
                return false; // Não é associado
            }

            return true;
        }

        public async Task<EventoPessoaSolicitacaoDTO?> GetSolicitacaoAssociado(int idEvento, int idPessoa)
        {
            var query = from ep in _context.Eventopessoas
                        where ep.IdEvento == idEvento && ep.IdPessoa == idPessoa
                        select new EventoPessoaSolicitacaoDTO
                        {
                            IdEvento = ep.IdEvento,
                            IdPessoa = ep.IdPessoa,
                            IdTipoInstrumento = ep.IdTipoInstrumento,
                            NomeInstrumento = ep.IdTipoInstrumentoNavigation != null
                                            ? ep.IdTipoInstrumentoNavigation.Nome
                                            : null,
                            Status = ep.Status,
                            StatusEnum = ConvertAprovadoParaEnum(ep.Status)
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
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
                                else if (auxSolicitacaoEvento[i].Aprovado == InscricaoEventoPessoa.INDEFERIDO || auxSolicitacaoEvento[i].Aprovado == InscricaoEventoPessoa.INSCRITO)
                                {
                                    auxAt.QuantidadeConfirmada++;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    return EventoStatus.ErroGenerico;
                                }
                                if (auxAt.QuantidadeConfirmada < 0)
                                {
                                    transaction.Rollback();
                                    return EventoStatus.QuantidadeConfirmadaNegativa;
                                }
                                if (auxAt.QuantidadeConfirmada > auxAt.QuantidadePlanejada)
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
        public async Task<IEnumerable<int>> GetIdRegentesEventoAsync(int idEvento)
{
    return await _context.Eventopessoas
                         .Where(ep => ep.IdEvento == idEvento && ep.IdPapelGrupoPapelGrupo == 5)
                         .Select(ep => ep.IdPessoa)
                         .ToListAsync();
}

// Adicione este novo método
public EventoDetailsDTO? GetDetails(int idEvento)
{
    var query = from evento in _context.Eventos
                where evento.Id == idEvento
                select new EventoDetailsDTO
                {
                    Id = evento.Id,
                    DataHoraInicio = evento.DataHoraInicio,
                    DataHoraFim = evento.DataHoraFim,
                    Local = evento.Local,
                    Repertorio = evento.Repertorio,
                    Regentes = (from eventopessoa in _context.Eventopessoas
                                join pessoa in _context.Pessoas on eventopessoa.IdPessoa equals pessoa.Id
                                where eventopessoa.IdEvento == idEvento && eventopessoa.IdPapelGrupoPapelGrupo == 5
                                select pessoa.Nome).ToList()
                };

    return query.FirstOrDefault();
}
        public IEnumerable<InstrumentoPlanejadoEventoDTO> GetInstrumentosPlanejadosEvento(int idApresentacao)
        {
            try
            {
                var query = from a in _context.Apresentacaotipoinstrumentos
                            join tp in _context.Tipoinstrumentos on a.IdTipoInstrumento equals tp.Id
                            where a.IdApresentacao == idApresentacao
                            select new InstrumentoPlanejadoEventoDTO
                            {
                                IdApresentacao = a.IdApresentacao,
                                IdInstrumento = a.IdTipoInstrumento,
                                ListaInstrumentos = tp.Nome ?? "INSTRUMENTO INVÁLIDO/REMOVIDO",
                                Planejados = a.QuantidadePlanejada,
                                Solicitados = a.QuantidadeSolicitada,
                                Confirmados = a.QuantidadeConfirmada
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna lista vazia
                return new List<InstrumentoPlanejadoEventoDTO>();
            }
        }

        // MÉTODO ADICIONAL: Para verificar se existem registros na tabela ApresentacaoTipoInstrumento
        public bool VerificarInstrumentosPlanejados(int idApresentacao)
        {
            try
            {
                var count = _context.Apresentacaotipoinstrumentos.Count(a => a.IdApresentacao == idApresentacao);
                Console.WriteLine($"Total de registros na ApresentacaoTipoInstrumento para evento {idApresentacao}: {count}");
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar instrumentos: {ex.Message}");
                return false;
            }
        }

        public IEnumerable<EventoAssociadoDTO>? GetEventosDeAssociado(int idPessoa, int idGrupoMusical, int PegarUltimosEventoDeAssociado)
        {
            DateTime pegarUltimosMeses = DateTime.Now.AddMonths(PegarUltimosEventoDeAssociado);
            var query = (from evento in _context.Eventos
                         where idGrupoMusical == evento.IdGrupoMusical && evento.DataHoraInicio.Date >= pegarUltimosMeses.Date
                         select new EventoAssociadoDTO
                         {
                             Id = evento.Id,
                             IdGrupoMusical = idGrupoMusical,
                             Local = evento.Local,
                             Inicio = evento.DataHoraInicio,
                             Fim = evento.DataHoraFim,
                             AprovadoModel = ConvertAprovadoParaEnum(
                                 _context.Eventopessoas.
                                 Where(ep => ep.IdEvento == evento.Id && ep.IdPessoa == idPessoa)
                                 .Select(ep => ep.Status).AsNoTracking().FirstOrDefault()),
                         }).AsNoTracking().ToList();
            return query;
        }

        /// <summary>
        /// Converte o status string para o enum InscricaoEventoPessoa
        /// </summary>
        /// <param name="status">Status como string vindo do banco</param>
        /// <returns>Enum correspondente</returns>
        private static InscricaoEventoPessoa ConvertAprovadoParaEnum(string? status)
        {
            if (string.IsNullOrEmpty(status))
                return InscricaoEventoPessoa.NAO_SOLICITADO;

            return status.ToUpper() switch
            {
                "INSCRITO" => InscricaoEventoPessoa.INSCRITO,
                "DEFERIDO" => InscricaoEventoPessoa.DEFERIDO,
                "INDEFERIDO" => InscricaoEventoPessoa.INDEFERIDO,
                _ => InscricaoEventoPessoa.NAO_SOLICITADO
            };
        }

    }
}