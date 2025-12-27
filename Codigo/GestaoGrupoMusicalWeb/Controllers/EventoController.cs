using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Core.Datatables;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using Service;
using Core.DTO;
using System.Security.Claims;
using System.Configuration;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : BaseController
    {
        private readonly IEventoService _eventoService;
        private readonly IEnsaioService _ensaioService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IPessoaService _pessoaService;
        private readonly IFigurinoService _figurinoService;
        private readonly IInstrumentoMusicalService _tipoIntrumentoMusicalService;
        
        private int FaltasPessoasEmEnsaioMeses { get; }

        public EventoController(
            IEventoService evento, 
            IEnsaioService ensaioService, 
            IMapper mapper, 
            IGrupoMusicalService grupoMusical, 
            IPessoaService pessoaService, 
            IFigurinoService figurino, 
            IInstrumentoMusicalService tipoInstrumentoMusical,
            IConfiguration configuration,
            ILogger<BaseController> logger)
                : base(logger)
        {
            _eventoService = evento ?? throw new ArgumentNullException(nameof(evento));
            _ensaioService = ensaioService;
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _pessoaService = pessoaService;
            _figurinoService = figurino;
            _tipoIntrumentoMusicalService = tipoInstrumentoMusical;
            FaltasPessoasEmEnsaioMeses = configuration.GetValue<int>("Aplication:FaltasPessoasEmEnsaioEmMeses");        }

        // GET: EventoController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var response = _eventoService.GetDataPage(request, await _grupoMusicalService.GetIdGrupo(User.Identity.Name));
            return Json(response);
        }

        // GET: EventoController/Details/5
        public ActionResult Details(int id)
        {
            var evento = _eventoService.GetDetails(id);
            if (evento == null)
            {
                Notificar("Evento não encontrado!", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: EventoController/Create
        public async Task<ActionResult> Create()
        {

            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = _pessoaService.GetRegentesForAutoComplete(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var figurinosDropdown = await _figurinoService.GetAllFigurinoDropdown(idGrupoMusical);

            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            EventoCreateViewlModel eventoModelCreate = new()
            {
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome")
            };

            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            eventoModelCreate.JsonLista = listaPessoasAutoComplete.ToJson();
            return View(eventoModelCreate);
        }

        // POST: EventoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventoCreateViewlModel eventoModel)
        {

            if (ModelState.IsValid && eventoModel.IdRegentes != null)
            {
                int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
                eventoModel.IdGrupoMusical = idGrupoMusical;

                int auxIdPessoa = (await _pessoaService.GetByCpf(User.Identity.Name))?.Id ?? 0;
                if (auxIdPessoa != 0 && eventoModel.IdFigurinoSelecionado != 0)
                {
                    Evento evento = _mapper.Map<Evento>(eventoModel);
                    evento.IdColaboradorResponsavel = auxIdPessoa;
                    string mensagem = "";
                    switch (await _eventoService.Create(evento, eventoModel.IdRegentes, eventoModel.IdFigurinoSelecionado))
                    {
                        case HttpStatusCode.OK:
                            Notificar("Evento <b>Cadastrado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                            break;
                        case HttpStatusCode.BadRequest:
                            mensagem = "Alerta! A <b>data de início</b> deve ser maior que a data de <b>hoje</b>";
                            Notificar(mensagem, Notifica.Alerta);
                            break;
                        case HttpStatusCode.PreconditionFailed:
                            mensagem = "Alerta! A data de <b>início</b> deve ser menor que a data <b>fim</b> " + DateTime.Now;
                            Notificar(mensagem, Notifica.Alerta);
                            break;
                        default:
                            mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do evento.";
                            Notificar(mensagem, Notifica.Erro);
                            break;
                    }
                }
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado ao cadastrar um novo Evento", Notifica.Erro);
            }            
            return RedirectToAction(nameof(Index));
        }

        // GET: EventoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var evento = _eventoService.Get(id);
            if (evento == null)
            {
                Notificar("Evento <b>não</b> encontrado.", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
            var listaPessoasAutoComplete = _pessoaService.GetRegentesForAutoComplete(evento.IdGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var figurinosDropdown = await _figurinoService.GetAllFigurinoDropdown(evento.IdGrupoMusical);

            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            EventoCreateViewlModel eventoModelCreate = new()
            {
                Id = evento.Id,
                IdGrupoMusical = evento.IdGrupoMusical,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                Local = evento.Local,
                Repertorio = evento.Repertorio,
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome")
            };

            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            eventoModelCreate.JsonLista = listaPessoasAutoComplete.ToJson();
            return View(eventoModelCreate);
        }

        // POST: EventoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EventoCreateViewlModel eventoModel)
        {
            if (ModelState.IsValid && eventoModel.IdFigurinoSelecionado != 0 && eventoModel.IdRegentes != null && eventoModel.IdRegentes.Any())
            {
                var colaborador = await _pessoaService.GetByCpf(User.Identity?.Name);
                if (colaborador == null)
                {
                    return RedirectToAction("Sair", "Identity");
                }

                var evento = _mapper.Map<Evento>(eventoModel);
                evento.IdColaboradorResponsavel = colaborador.Id;
                evento.IdGrupoMusical = colaborador.IdGrupoMusical;

                // Chamada ao novo método de serviço
                switch (_eventoService.Edit(evento, eventoModel.IdRegentes, eventoModel.IdFigurinoSelecionado))
                {
                    case HttpStatusCode.OK:
                        Notificar("Evento <b>Editado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case HttpStatusCode.BadRequest:
                        Notificar("Alerta! A <b>data de início</b> deve ser maior que a data de <b>hoje</b>", Notifica.Alerta);
                        break;
                    case HttpStatusCode.PreconditionFailed:
                        Notificar("Alerta! A data de <b>início</b> deve ser menor que a data <b>fim</b> ", Notifica.Alerta);
                        break;
                    default:
                        Notificar("<b>Erro</b>! Desculpe, ocorreu um erro durante a <b>Edição</b> do evento.", Notifica.Erro);
                        break;
                }
            }
            else
            {
                Notificar("<b>Erro</b>! Verifique os dados do formulário.", Notifica.Erro);
            }
            // Recarregar os dados necessários para a view em caso de erro
            var listaPessoasAutoComplete = _pessoaService.GetRegentesForAutoComplete(eventoModel.IdGrupoMusical);
            var figurinosDropdown = await _figurinoService.GetAllFigurinoDropdown(eventoModel.IdGrupoMusical);
            eventoModel.ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome");
            eventoModel.FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome");
            return View(eventoModel);
        }

        // POST: EventoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            HttpStatusCode result = _eventoService.Delete(id);
            switch (result)
            {
                case HttpStatusCode.OK:
                    Notificar("Evento <b>Deletado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("Evento <b>não</b> encontrado.", Notifica.Alerta);
                    break;
                default:
                    Notificar($"O <b>Evento</b> não pôde ser <b>deletado</b>. Consulte o suporte para detalhes.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> NotificarEventoViaEmail(int id)
        {
            var pessoas = await _grupoMusicalService.GetAllPeopleFromGrupoMusical(await _grupoMusicalService.GetIdGrupo(User.Identity.Name));
            switch (_eventoService.NotificarEventoViaEmail(pessoas, id))
            {
                case HttpStatusCode.OK:
                    Notificar("Notificação de Evento foi <b>Enviada</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("O Evento <b>Não</b> está <b>Cadastrado</b> no sistema.", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar($"O evento {id} <b>não foi encontrado</b>.", Notifica.Erro);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("Houve um erro. Tente novamente mais tarde.", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Envio</b> da notificação.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: EventoController/Edit/5

        public async Task<ActionResult> GerenciarInstrumentoEvento(int id)
        {
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = _pessoaService.GetRegentesForAutoComplete(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var figurinosDropdown = await _figurinoService.GetAllFigurinoDropdown(idGrupoMusical);
            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var evento = _eventoService.Get(id);
            if (evento == null)
            {
                Notificar("Evento não encontrado.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }

            var eventoView = _mapper.Map<EventoViewModel>(evento);
            var listaInstrumentosDropdown = await _tipoIntrumentoMusicalService.GetAllTipoInstrumento();

            // Buscar os instrumentos planejados
            var instrumentosPlanejados = _eventoService.GetInstrumentosPlanejadosEvento(id);

            var gerenciarInstrumentoEventoViewModel = new GerenciarInstrumentoEventoViewModel
            {
                Id = evento.Id,
                IdGrupoMusical = idGrupoMusical,
                DataHoraInicio = eventoView.DataHoraInicio,
                DataHoraFim = eventoView.DataHoraFim,
                Local = eventoView.Local,
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome"),
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                ListaInstrumentos = new SelectList(listaInstrumentosDropdown, "Id", "Nome"),
                InstrumentosPlanejados = instrumentosPlanejados,
                GerenciarInstrumentos = instrumentosPlanejados
            };

            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            gerenciarInstrumentoEventoViewModel.JsonLista = listaPessoasAutoComplete.ToJson();

            return View(gerenciarInstrumentoEventoViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateInstrumento(GerenciarInstrumentoEventoViewModel gerenciarInstrumentoEventoViewModel)
        {
            if (gerenciarInstrumentoEventoViewModel.Id <= 0)
            {
                Notificar("<b>Erro!</b> ID do Evento não foi encontrado no formulário.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }

            Apresentacaotipoinstrumento apresentacaotipoinstrumento = new Apresentacaotipoinstrumento
            {
                IdApresentacao = gerenciarInstrumentoEventoViewModel.Id,
                IdTipoInstrumento = gerenciarInstrumentoEventoViewModel.IdTipoInstrumento,
                QuantidadePlanejada = gerenciarInstrumentoEventoViewModel.Quantidade
            };

            switch (await _eventoService.CreateApresentacaoInstrumento(apresentacaotipoinstrumento))
            {
                case HttpStatusCode.OK:
                    Notificar("Instrumento(s) Planejado(s) <b>Cadastrado(s)</b> com <b>Sucesso!</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.Conflict:
                    Notificar("<b>Alerta!</b> Este instrumento já foi adicionado.", Notifica.Alerta);
                    break;
                default:
                    Notificar("<b>Erro!</b> Desculpe, ocorreu um erro durante o <b>Cadastro</b> do instrumento.", Notifica.Erro);
                    break;
            }

            return RedirectToAction(nameof(GerenciarInstrumentoEvento), new { id = gerenciarInstrumentoEventoViewModel.Id });
        }

        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> SolicitarParticipacao(int id)
        {
            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);

            if (!await _eventoService.PodeAssociadoSolicitar(id, idPessoa))
            {
                Notificar("Não é possível solicitar participação neste evento.", Notifica.Alerta);
                return RedirectToAction("MeusEventos");
            }

            var evento = _eventoService.Get(id);
            if (evento == null)
            {
                Notificar("Evento não encontrado.", Notifica.Erro);
                return RedirectToAction("MeusEventos");
            }

            var instrumentosDisponiveis = _eventoService.GetInstrumentosDisponiveis(id);
            var minhaInscricao = await _eventoService.GetSolicitacaoAssociado(id, idPessoa);

            var model = new EventoDetalhesAssociadoDTO
            {
                Id = evento.Id,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                Local = evento.Local,
                Repertorio = evento.Repertorio,
                InstrumentosDisponiveis = instrumentosDisponiveis,
                MinhaInscricao = minhaInscricao,
                PodeInscrever = minhaInscricao == null || minhaInscricao.StatusEnum == InscricaoEventoPessoa.NAO_SOLICITADO,
                PodeCancelar = minhaInscricao?.StatusEnum == InscricaoEventoPessoa.INSCRITO
            };

            return View(model);
        }

        [Authorize(Roles = "ASSOCIADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmarSolicitacao(SolicitarParticipacaoDTO solicitacao)
        {
            if (!ModelState.IsValid)
            {
                Notificar("Dados inválidos. Verifique os campos obrigatórios.", Notifica.Alerta);
                return RedirectToAction("SolicitarParticipacao", new { id = solicitacao.IdEvento });
            }

            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);

            var resultado = await _eventoService.SolicitarParticipacao(
                solicitacao.IdEvento,
                idPessoa,
                solicitacao.IdTipoInstrumento
            );

            switch (resultado)
            {
                case HttpStatusCode.OK:
                    Notificar("Solicitação de participação enviada com <b>sucesso</b>!", Notifica.Sucesso);
                    break;
                case HttpStatusCode.Conflict:
                    Notificar("Você já solicitou participação com este instrumento.", Notifica.Alerta);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("Não é possível solicitar participação neste momento.", Notifica.Alerta);
                    break;
                default:
                    Notificar("Erro ao processar sua solicitação. Tente novamente.", Notifica.Erro);
                    break;
            }

            // CORREÇÃO: Redirecionar de volta para a página de solicitação para mostrar status atualizado
            return RedirectToAction("SolicitarParticipacao", new { id = solicitacao.IdEvento });
        }

        [Authorize(Roles = "ASSOCIADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelarSolicitacao(int idEvento)
        {

            var claimId = User.FindFirst("Id")?.Value;

            if (string.IsNullOrEmpty(claimId))
            {
                Notificar("Erro: Identificador numérico não encontrado no perfil.", Notifica.Erro);
                return RedirectToAction("Index","Home");
            }
          
            int idPessoa = Convert.ToInt32(claimId);

            var resultado = await _eventoService.CancelarSolicitacao(idEvento, idPessoa);

            switch (resultado)
            {
                case HttpStatusCode.OK:
                    Notificar("Solicitação cancelada com <b>sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("Solicitação não encontrada.", Notifica.Alerta);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("Não é possível cancelar esta solicitação.", Notifica.Alerta);
                    break;
                default:
                    Notificar("Erro ao cancelar solicitação. Tente novamente.", Notifica.Erro);
                    break;
            }

            // CORREÇÃO: Redirecionar de volta para a página de solicitação
            return RedirectToAction("SolicitarParticipacao", new { id = idEvento });
        }

        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> MeusEventos()
        {
            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            // Buscar eventos dos últimos 6 meses
            var eventos = _eventoService.GetEventosDeAssociado(idPessoa, idGrupoMusical, -6);

            return View(eventos);
        }

        [Authorize(Roles = "ASSOCIADO")]
        public JsonResult GetInstrumentosDisponiveis(int idEvento)
        {
            var instrumentos = _eventoService.GetInstrumentosDisponiveis(idEvento);
            return Json(instrumentos);
        }

        // MÉTODO para administradores verificarem status das solicitações
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult StatusSolicitacoes(int id)
        {
            var statusGeral = _eventoService.GetInstrumentosPlanejadosEvento(id);
            return View(statusGeral);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult GerenciarSolicitacaoEvento(int id)
        {
            GerenciarSolicitacaoEventoDTO? g = _eventoService.GetSolicitacoesEventoDTO(id, FaltasPessoasEmEnsaioMeses);
            GerenciarSolicitacaoEventoViewModel? model = _mapper.Map<GerenciarSolicitacaoEventoViewModel>(g);
            return View(model);
        }

        public ActionResult GerenciarSolicitacaoEventoModel(GerenciarSolicitacaoEventoViewModel model)
        {
            GerenciarSolicitacaoEventoDTO? g = _mapper.Map<GerenciarSolicitacaoEventoDTO>(model);

            switch(_eventoService.EditSolicitacoesEvento(g))
            {
                case IEventoService.EventoStatus.Success:
                    Notificar("Solicitação de <b>participação</b> do evento alterado com <b>sucesso</b>.", Notifica.Sucesso);
                    break;
                case IEventoService.EventoStatus.SemAlteracao:
                    Notificar("<b>Alerta!</b> Não houve alterações na solicitação de participação do evento dos associados.", Notifica.Informativo);
                    break;
                case IEventoService.EventoStatus.QuantidadeSolicitadaNegativa:
                    Notificar("<b>Erro!</b> Solicitação de participação do evento está <b>negativa</b>. Consulte o <b>suporte</b>.", Notifica.Erro);
                    break;
                case IEventoService.EventoStatus.UltrapassouLimiteQuantidadePlanejada:
                    Notificar("<b>Erro!</b> Ultrapassou o <b>limite</b> de participação de associados em um determinado <b>instrumento</b>", Notifica.Erro);
                    break;
                default:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante o geremciamento de <b>solicitação</b> dos associados.", Notifica.Erro);
                    break;
            }
            
            return RedirectToAction(nameof(GerenciarSolicitacaoEvento), new { id = model.Id });
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EventoController/RegistrarFrequencia
        public async Task<ActionResult> RegistrarFrequencia(int id)
        {
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            // MODIFICAÇÃO: Busca os dados de frequência já filtrados
            var eventoFrequenciaData = await _eventoService.GetFrequenciaAsync(id, idGrupoMusical);
            if (eventoFrequenciaData == null)
            {
                Notificar("Evento não encontrado ou sem participantes aprovados.", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }

            var evento = _eventoService.Get(id);
            if (evento == null)
            {
                Notificar("Evento não encontrado.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }

            var listaRegentes = _pessoaService.GetRegentesForAutoComplete(idGrupoMusical);
            var listaFigurinos = await _figurinoService.GetAllFigurinoDropdown(idGrupoMusical);

            EventoViewModel eventoView = _mapper.Map<EventoViewModel>(evento);

            // MODIFICAÇÃO: Atribui a lista correta de associados ao ViewModel
            eventoView.Frequencias = eventoFrequenciaData.Frequencias;

            eventoView.ListaPessoa = new SelectList(listaRegentes, "Id", "Nome");
            eventoView.ListaFigurino = new SelectList(listaFigurinos, "Id", "Nome");

            ViewData["exemploRegente"] = listaRegentes?.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            ViewData["jsonIdRegentes"] = (await _eventoService.GetIdRegentesEventoAsync(eventoView.Id)).ToJson();
            eventoView.JsonLista = listaRegentes.ToJson();

            return View(eventoView);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarFrequencia(List<EventoListaFrequenciaDTO> listaFrequencia)
        {
            switch (await _eventoService.RegistrarFrequenciaAsync(listaFrequencia))
            {
                case HttpStatusCode.OK:
                    Notificar("Lista de <b>Frequência</b> salva com <b>Sucesso</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("A <b>Lista</b> enviada <b>Não</b> possui registros", Notifica.Alerta);
                    return RedirectToAction(nameof(Index));
                case HttpStatusCode.Conflict:
                    Notificar("A <b>Lista</b> enviada é <b>Inválida</b>", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("A <b>Lista</b> enviada não foi <b>Encontrada</b>", Notifica.Erro);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> ao registrar a Lista de <b>Frequência</b>.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(RegistrarFrequencia), new { idEvento = listaFrequencia.First().IdEvento });
        }


        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> JustificarAusencia(int idEvento)
        {
            var model = await _eventoService.GetEventoPessoaAsync(idEvento, Convert.ToInt32(User.FindFirst("Id")?.Value));

            EventoJustificativaViewModel eventoJustificativa = new()
            {
                IdEvento = model.IdEvento,
                Justificativa = model.JustificativaFalta
            };
            return View(eventoJustificativa);
        }
        [Authorize(Roles = "ASSOCIADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JustificarAusencia(EventoJustificativaViewModel eventoJustificativa)
        {
            if (ModelState.IsValid)
            {
                switch (await _eventoService.RegistrarJustificativaAsync(eventoJustificativa.IdEvento, Convert.ToInt32(User.FindFirst("Id")?.Value), eventoJustificativa.Justificativa))
                {
                    case HttpStatusCode.OK:
                        Notificar("<b>Justificativa</b> registrada com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case HttpStatusCode.NotFound:
                        Notificar("A <b>Justificativa</b> enviada é <b>Inválida</b>", Notifica.Erro);
                        break;
                    case HttpStatusCode.Unauthorized:
                        Notificar("Desculpe, <b>Não</b> foi possível <b>Registrar</b> a <b>Justificativa</b>", Notifica.Erro);
                        break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("Desculpe, ocorreu um <b>Erro</b> ao registrar a <b>Justificativa</b>, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        break;
                }
            }

            return View(eventoJustificativa);
        }
        public IActionResult GerenciarInstrumentos(int idApresentacao)
        {

            //var instrumentos = await _eventoService.GetInstrumentosPlanejadosEvento(idApresentacao);

            //            InstrumentoPlanejadoEventoDTO? g = _eventoService.GetInstrumentosPlanejadosEvento(idApresentacao);
            IEnumerable<InstrumentoPlanejadoEventoDTO> instrumentos = _eventoService.GetInstrumentosPlanejadosEvento(idApresentacao);
            

            Console.WriteLine("Teste instrumentos: " + instrumentos);

            return View(instrumentos);
        }
    }
}
