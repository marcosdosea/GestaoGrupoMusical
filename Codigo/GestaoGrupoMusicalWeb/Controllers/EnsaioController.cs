using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class EnsaioController : Controller
    {
        private readonly IEnsaio _ensaio;
        private readonly IMapper _mapper;

        public EnsaioController(IMapper mapper, IEnsaio ensaio)
        {
            _ensaio = ensaio;
            _mapper = mapper;
        }

        // GET: EnsaioController
        public ActionResult Index()
        {
            var ensaios = _ensaio.GetAll();
            return View(_mapper.Map<IAsyncEnumerable<EnsaioViewModel>>(ensaios));
        }

        // GET: EnsaioController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        // GET: EnsaioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EnsaioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EnsaioViewModel ensaioModel)
        {
            if (ModelState.IsValid)
            {
                if (await _ensaio.Create(_mapper.Map<Ensaio>(ensaioModel))) 
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(ensaioModel);
        }

        // GET: EnsaioController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        // POST: EnsaioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnsaioViewModel ensaioModel)
        {
            if (ModelState.IsValid)
            {
                if (await _ensaio.Edit(_mapper.Map<Ensaio>(ensaioModel)))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(ensaioModel);
        }

        // GET: EnsaioController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        // POST: EnsaioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(EnsaioViewModel ensaioModel)
        {
            await _ensaio.Delete(ensaioModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
