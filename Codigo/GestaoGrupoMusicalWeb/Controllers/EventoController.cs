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

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : BaseController
    {
        private readonly IEventoService _evento;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IPessoaService _pessoaService;
        private readonly IFigurinoService _figurino;
        private readonly IInstrumentoMusicalService _tipoIntrumentoMusical;



        public EventoController(IEventoService evento, IMapper mapper, IGrupoMusicalService grupoMusical, IPessoaService pessoaService, IFigurinoService figurino, IInstrumentoMusicalService tipoInstrumentoMusical)
        {
            _evento = evento;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _pessoaService = pessoaService;
            _figurino = figurino;
            _tipoIntrumentoMusical = tipoInstrumentoMusical;
        }

        // GET: EventoController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var response = _evento.GetDataPage(request, await _grupoMusical.GetIdGrupo(User.Identity.Name));
            return Json(response);
        }

        // GET: EventoController/Details/5
        public ActionResult Details(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            return View(eventoModel);
        }

        // GET: EventoController/Create
        public async Task<ActionResult> Create()
        {

            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = await _pessoaService.GetRegentesForAutoCompleteAsync(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var figurinosDropdown = await _figurino.GetAllFigurinoDropdown(idGrupoMusical);

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
                int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);
                eventoModel.IdGrupoMusical = idGrupoMusical;

                int auxIdPessoa = (await _pessoaService.GetByCpf(User.Identity.Name))?.Id ?? 0;
                if (auxIdPessoa != 0 && eventoModel.IdFigurinoSelecionado != 0)
                {
                    Evento evento = _mapper.Map<Evento>(eventoModel);
                    evento.IdColaboradorResponsavel = auxIdPessoa;
                    string mensagem = "";
                    switch (await _evento.Create(evento, eventoModel.IdRegentes, eventoModel.IdFigurinoSelecionado))
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
            Console.WriteLine("View model");
            return RedirectToAction(nameof(Index));
        }

        // GET: EventoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var evento = _evento.Get(id);
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
            var figurinosDropdown = await _figurino.GetAllFigurinoDropdown(evento.IdGrupoMusical);

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
                switch (_evento.Edit(evento))
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
            HttpStatusCode result = _evento.Delete(id);
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
            var pessoas = await _grupoMusical.GetAllPeopleFromGrupoMusical(await _grupoMusical.GetIdGrupo(User.Identity.Name));
            switch (_evento.NotificarEventoViaEmail(pessoas, id))
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

            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);

            var listaPessoasAutoComplete = await _pessoaService.GetRegentesForAutoCompleteAsync(idGrupoMusical);
            if (listaPessoasAutoComplete == null || !listaPessoasAutoComplete.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var figurinosDropdown = await _figurino.GetAllFigurinoDropdown(idGrupoMusical);

            if (figurinosDropdown == null || !figurinosDropdown.Any())
            {

                Notificar("É necessário cadastrar um Figurino para então cadastrar um Evento Musical.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            var evento = _evento.Get(id);
            EventoViewModel eventoView = _mapper.Map<EventoViewModel>(evento);

            InstrumentoMusicalViewModel instrumentoMusicalViewModel = new InstrumentoMusicalViewModel();
            IEnumerable<Tipoinstrumento> listaInstrumentos = await _tipoIntrumentoMusical.GetAllTipoInstrumento();
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


    }
}
