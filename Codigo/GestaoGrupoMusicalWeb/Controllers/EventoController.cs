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
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IPessoaService _pessoaService;
        private readonly IFigurinoService _figurinoService;
        private readonly IInstrumentoMusicalService _tipoIntrumentoMusicalService;
        
        private int FaltasPessoasEmEnsaioMeses { get; }


        public EventoController(IEventoService evento, IMapper mapper, 
            IGrupoMusicalService grupoMusical, IPessoaService pessoaService, 
            IFigurinoService figurino, IInstrumentoMusicalService tipoInstrumentoMusical,
            IConfiguration configuration)
        {
            _eventoService = evento;
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _pessoaService = pessoaService;
            _figurinoService = figurino;
            _tipoIntrumentoMusicalService = tipoInstrumentoMusical;
            FaltasPessoasEmEnsaioMeses = configuration.GetValue<int>("Aplication:FaltasPessoasEmEnsaioEmMeses");
        }

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
            var evento = _eventoService.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            

            return View(eventoModel);
        }

        // GET: EventoController/Create
        public async Task<ActionResult> Create()
        {

            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = await _pessoaService.GetRegentesForAutoCompleteAsync(idGrupoMusical);
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
            var listaPessoasAutoComplete = await _pessoaService.GetRegentesForAutoCompleteAsync(evento.IdGrupoMusical);
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
                if (eventoModel.IdFigurinoSelecionado != 0 && eventoModel.IdRegentes != null)
                {
                    foreach (int idPessoa in eventoModel.IdRegentes)
                    {
                        evento.Eventopessoas.Add(new Eventopessoa()
                        {
                            IdPessoa = idPessoa,
                            IdEvento = evento.Id,
                            IdPapelGrupoPapelGrupo = 5,
                            IdTipoInstrumento = 0
                        });
                    }
                    evento.IdFigurinos.Add(new Figurino() { Id = eventoModel.IdFigurinoSelecionado });
                }
                switch (_eventoService.Edit(evento))
                {
                    case HttpStatusCode.OK:
                        Notificar("Evento <b>Editado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        break;
                    case HttpStatusCode.BadRequest:
                        Notificar("Alerta! A <b>data de início</b> deve ser maior que a data de <b>hoje</b>", Notifica.Alerta);
                        break;
                    case HttpStatusCode.PreconditionFailed:
                        Notificar("Alerta! A data de <b>início</b> deve ser menor que a data <b>fim</b> ", Notifica.Alerta);
                        break;
                    default:
                        Notificar("<b>Erro</b>! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do evento.", Notifica.Erro);
                        break;
                }
                return RedirectToAction(nameof(Index));
            }
            eventoModel.ListaPessoa = new SelectList(_pessoaService.GetAll(), "Id", "Nome");
            return RedirectToAction("Edit", eventoModel);
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

            var listaPessoasAutoComplete = await _pessoaService.GetRegentesForAutoCompleteAsync(idGrupoMusical);
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
            EventoViewModel eventoView = _mapper.Map<EventoViewModel>(evento);                       

            InstrumentoMusicalViewModel instrumentoMusicalViewModel = new InstrumentoMusicalViewModel();

            IEnumerable<Tipoinstrumento> listaInstrumentos = await _tipoIntrumentoMusicalService.GetAllTipoInstrumento();
            instrumentoMusicalViewModel.ListaInstrumentos = new SelectList(listaInstrumentos, "Id", "Nome", null);

            GerenciarInstrumentoEventoViewModel gerenciarInstrumentoEvento = new GerenciarInstrumentoEventoViewModel
            {
                IdGrupoMusical = idGrupoMusical,
                DataHoraInicio = eventoView.DataHoraInicio,
                DataHoraFim = eventoView.DataHoraFim,
                ListaPessoa = new SelectList(listaPessoasAutoComplete, "Id", "Nome"),
                FigurinoList = new SelectList(figurinosDropdown, "Id", "Nome"),                              
                Local = eventoView.Local,
                ListaInstrumentos = instrumentoMusicalViewModel.ListaInstrumentos,
            };
            
            ViewData["exemploRegente"] = listaPessoasAutoComplete.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            gerenciarInstrumentoEvento.JsonLista = listaPessoasAutoComplete.ToJson();
            return View(gerenciarInstrumentoEvento);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateInstrumento(GerenciarInstrumentoEventoViewModel gerenciarInstrumentoEventoViewModel)
        {           
            Apresentacaotipoinstrumento apresentacaotipoinstrumento = new Apresentacaotipoinstrumento
            {
                IdApresentacao = gerenciarInstrumentoEventoViewModel.Id,
                IdTipoInstrumento = gerenciarInstrumentoEventoViewModel.IdTipoInstrumento,
                QuantidadePlanejada = gerenciarInstrumentoEventoViewModel.Quantidade                             
            };
           
            switch (await _eventoService.CreateApresentacaoInstrumento(apresentacaotipoinstrumento))
            {
                case HttpStatusCode.OK:
                    Notificar("Instrumento(s) Planejado(os) <b>Cadastrado(s)</b> com <b>Sucesso!</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.Conflict:
                    Notificar("<b>Erro!</b> já existe um instrumento planejado, clique no botão editar para adicionar atualizar a quantidade.", Notifica.Alerta);
                    break;
                default:
                    Notificar("<b>Erro!</b> Desculpe, ocorreu um erro durante o <b>Cadastro</b> do instrumento.", Notifica.Erro);
                    break;
            }



            return RedirectToAction(nameof(GerenciarInstrumentoEvento), new { id = apresentacaotipoinstrumento.IdApresentacao });
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

            var listaRegentes = await _pessoaService.GetRegentesForAutoCompleteAsync(idGrupoMusical);

            if (listaRegentes == null || !listaRegentes.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var listaFigurinos = await _figurinoService.GetAllFigurinoDropdown(idGrupoMusical);

            if (listaFigurinos == null || !listaFigurinos.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Figurino para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var listaAssociadosAtivos = await _pessoaService.GetAssociadoAtivos(idGrupoMusical);

            if (listaAssociadosAtivos == null || !listaAssociadosAtivos.Any())
            {
                Notificar("É necessário pelo menos um Associado Ativo para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var evento = _eventoService.Get(id);

            EventoViewModel eventoView = _mapper.Map<EventoViewModel>(evento);

            eventoView.ListaPessoa = new SelectList(listaRegentes, "Id", "Nome");
            eventoView.ListaFigurino = new SelectList(listaFigurinos, "Id", "Nome");
            eventoView.AssociadosDTO = listaAssociadosAtivos;

            ViewData["exemploRegente"] = listaRegentes.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
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
    }
}
