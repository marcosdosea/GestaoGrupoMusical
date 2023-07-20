using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : Controller
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
        public  ActionResult Create(EventoViewModel eventoModel)
        {

            int idGrupo = _grupoMusical.GetByIdGrupo(User.Identity.Name);
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
            eventoModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");
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
                _evento.Create(evento);
            }
            eventoModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");
            eventoModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome"); 
            return RedirectToAction(nameof(Index));
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
    }
}
