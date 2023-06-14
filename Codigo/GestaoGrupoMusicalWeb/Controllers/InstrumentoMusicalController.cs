using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InstrumentoMusicalController : Controller
    {
        private readonly IInstrumentoMusicalService _instrumentoMusical;
        private readonly IPessoaService _pessoa;
        private readonly IMapper _mapper;

        public InstrumentoMusicalController(IInstrumentoMusicalService instrumentoMusical, IPessoaService pessoa, IMapper mapper)
        {
            _instrumentoMusical = instrumentoMusical;
            _pessoa = pessoa;
            _mapper = mapper;

        }

        // GET: InstrumentoMusicalController
        public async Task<ActionResult> Index()
        {
            var listaInstrumentoMusical = await _instrumentoMusical.GetAllDTO();
            return View(listaInstrumentoMusical);
        }


        // GET: InstrumentoMusicalController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
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
        public async Task<ActionResult> Create(InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            if (ModelState.IsValid)
            {
                var instrumentoMusicalModel = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                await _instrumentoMusical.Create(instrumentoMusicalModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);

            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            if (ModelState.IsValid)
            {
                var instrumentoMusical = _mapper.Map<Instrumentomusical>(instrumentoMusicalViewModel);
                await _instrumentoMusical.Edit(instrumentoMusical);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InstrumentoMusicalController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var instrumentoMusical = await _instrumentoMusical.Get(id);
            var instrumentoMusicalModel = _mapper.Map<InstrumentoMusicalViewModel>(instrumentoMusical);
            return View(instrumentoMusicalModel);
        }

        // POST: InstrumentoMusicalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, InstrumentoMusicalViewModel instrumentoMusicalViewModel)
        {
            await _instrumentoMusical.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Movimentar(int id)
        {
            MovimentacaoInstrumentoViewModel movimentacao = new();
            var instrumento = await _instrumentoMusical.Get(id);
            movimentacao.Patrimonio = instrumento.Patrimonio;
            movimentacao.NomeInstrumento = await _instrumentoMusical.GetNomeInstrumento(id);
            movimentacao.ListaAssociado = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            return View(movimentacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Movimentar(MovimentacaoInstrumentoViewModel movimentacaoPost)
        {
            //movimentacao.ListaAssociado = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            return View(movimentacaoPost);
        }
    }
}
