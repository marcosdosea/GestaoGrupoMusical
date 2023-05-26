using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class ApresentacaoController : Controller
    {
        private readonly IApresentacao _apresentacao;
        private readonly IMapper _mapper;

        public ApresentacaoController(IApresentacao grupoMusical, IMapper mapper)
        {
            _apresentacao = grupoMusical;
            _mapper = mapper;
        }

        // GET: ApresentacaoController
        public ActionResult Index()
        {
            var listaGrupoMusical = _apresentacao.GetAll();
            var listaGrupoMusicalModel = _mapper.Map<List<ApresentacaoViewModel>>(listaGrupoMusical);
            return View(listaGrupoMusicalModel);
        }

        // GET: ApresentacaoController/Details/5
        public ActionResult Details(int id)
        {
            var apresentacao = _apresentacao.Get(id);
            var apresentacaoModel = _mapper.Map<ApresentacaoViewModel>(apresentacao);
            return View(apresentacaoModel);
        }

        // GET: ApresentacaoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApresentacaoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApresentacaoViewModel apresentacaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var apresentacaoModel = _mapper.Map<Apresentacao>(apresentacaoViewModel);
                _apresentacao.Create(apresentacaoModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ApresentacaoController/Edit/5
        public ActionResult Edit(int id)
        {
            var apresentacao = _apresentacao.Get(id);
            var apresentacaoModel = _mapper.Map<GrupoMusicalViewModel>(apresentacao);

            return View(apresentacaoModel);
        }

        // POST: ApresentacaoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ApresentacaoViewModel apresentacaoViewModel)
        {
            if (ModelState.IsValid)
            {
                var apresentacao = _mapper.Map<Apresentacao>(apresentacaoViewModel);
                _apresentacao.Edit(apresentacao);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ApresentacaoController/Delete/5
        public ActionResult Delete(int id)
        {
            var apresentacao = _apresentacao.Get(id);
            var apresentacaoModel = _mapper.Map<ApresentacaoViewModel>(apresentacao);
            return View(apresentacaoModel);
        }

        // POST: ApresentacaoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ApresentacaoViewModel apresentacaoViewModel)
        {
            _apresentacao.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
