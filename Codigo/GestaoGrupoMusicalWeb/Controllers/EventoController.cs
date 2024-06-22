using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using Core.Datatables;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : BaseController
    {
        private readonly IEventoService _evento;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IPessoaService _pessoa;


        public EventoController(IEventoService evento, IMapper mapper, IGrupoMusicalService grupoMusical, IPessoaService pessoa)
        {
            _evento = evento;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _pessoa = pessoa;
    }

        // GET: EventoController
        public ActionResult Index()
        {
            var listaEvento =  _evento.GetAllIndexDTO();
            return View(listaEvento);
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
        public ActionResult Create()
        {
            EventoViewModel eventoModel = new EventoViewModel
            {
                ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome"),
                ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome")
            };
            return View(eventoModel);
        }

        // POST: EventoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EventoViewModel eventoModel)
        {

            int idGrupo = await _grupoMusical.GetIdGrupo(User.Identity.Name);
            eventoModel.IdGrupoMusical = idGrupo;

            if (ModelState.IsValid)
            {
                var evento = _mapper.Map<Evento>(eventoModel);
                _evento.Create(evento);

            }
            eventoModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");

            return RedirectToAction(nameof(Index));
        }

        // GET: EventoController/Edit/5
        public ActionResult Edit(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome"); 

            return View(eventoModel);
        }

        // POST: EventoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventoViewModel eventoModel)
        {
            if (ModelState.IsValid)
            {
                var evento = _mapper.Map<Evento>(eventoModel);
                _evento.Edit(evento);
                return RedirectToAction(nameof(Index));
            }
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome"); 
            return RedirectToAction("Edit", eventoModel);
        }

        // GET: EventoController/Delete/5
        public ActionResult Delete(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);
            return View(eventoModel);
        }

        // POST: EventoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, EventoViewModel eventoViewModel)
        {
            _evento.Delete(id);
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
    }
}
