using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InstrumentoMusicalController : Controller
    {
        private readonly IInstrumentoMusicalService _instrumentoMusical;
        private readonly IMapper _mapper;

        public InstrumentoMusicalController(IInstrumentoMusicalService instrumentoMusical, IMapper mapper)
        {
            _instrumentoMusical = instrumentoMusical;
            _mapper = mapper;

        }

        // GET: InstrumentoMusicalController
        public ActionResult Index()
        {

            var listaInstrumentoMusical = _instrumentoMusical.GetAll();
            var listaInstrumentoMusicalModel = _mapper.Map<List<InstrumentoMusicalViewModel>>(listaInstrumentoMusical);
            return View(listaInstrumentoMusicalModel);

        }


        // GET: InstrumentoMusicalController/Details/5
        public ActionResult Details(int id)
        {
            var instrumentoMusical = _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // GET: InstrumentoMusicalController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InstrumentoMusicalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            if (ModelState.IsValid)
            {
                var instrumentoMusicalModel = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                _instrumentoMusical.Create(instrumentoMusicalModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Edit/5
        public ActionResult Edit(int id)
        {
            var instrumentoMusical = _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<EventoViewModel>(instrumentoMusical);

            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            if (ModelState.IsValid)
            {
                var instrumentoMusical = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                _instrumentoMusical.Edit(instrumentoMusical);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Delete/5
        public ActionResult Delete(int id)
        {
            var instrumentoMusical = _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<EventoViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            _instrumentoMusical.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
