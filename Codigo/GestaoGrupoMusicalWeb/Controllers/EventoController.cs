using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class EventoController : Controller
    {
        private readonly IEventoService _evento;
        private readonly IMapper _mapper;

        public EventoController(IEventoService evento, IMapper mapper)
        {
            _evento = evento;
            _mapper = mapper;
        }

        // GET: EventoController
        public ActionResult Index()
        {
            var listaEvento = _evento.GetAll();
            var listaEventoModel = _mapper.Map<List<EventoViewModel>>(listaEvento);
            return View(listaEventoModel);
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
            return View();
        }

        // POST: EventoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventoViewModel eventoViewModel)
        {
            if (ModelState.IsValid)
            {
                var eventoModel = _mapper.Map<Evento>(eventoViewModel);
                _evento.Create(eventoModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: EventoController/Edit/5
        public ActionResult Edit(int id)
        {
            var evento = _evento.Get(id);
            var eventoModel = _mapper.Map<EventoViewModel>(evento);

            return View(eventoModel);
        }

        // POST: EventoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventoViewModel eventoViewModel)
        {
            if (ModelState.IsValid)
            {
                var evento = _mapper.Map<Evento>(eventoViewModel);
                _evento.Edit(evento);
            }
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
