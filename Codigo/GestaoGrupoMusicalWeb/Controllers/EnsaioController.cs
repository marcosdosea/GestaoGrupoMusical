using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class EnsaioController : Controller
    {
        private readonly IEnsaioService _ensaio;
        private readonly IMapper _mapper;
        private readonly IPessoaService _pessoa;
        private readonly IGrupoMusical _grupoMusical;

        public EnsaioController(IMapper mapper, IEnsaioService ensaio, IPessoaService pessoa, IGrupoMusical grupoMusical)
        {
            _ensaio = ensaio;
            _mapper = mapper;
            _pessoa = pessoa;
            _grupoMusical = grupoMusical;
        }

        // GET: EnsaioController
        public ActionResult Index()
        {
            var ensaios = _ensaio.GetAll();
            return View(_mapper.Map<IEnumerable<EnsaioViewModel>>(ensaios));
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
            EnsaioViewModel ensaioModel = new();

            ensaioModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            ensaioModel.ListaGrupoMusical = new SelectList(_grupoMusical.GetAll(), "Id", "Nome");

            return View(ensaioModel);
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
