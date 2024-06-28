using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InformativoController : BaseController
    {
        // GET: InformativoController
        private readonly IInformativoService _informativoService;
        private readonly IPessoaService _pessoaService;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IMapper _mapper;

        public InformativoController(IInformativoService informativoService, IPessoaService pessoaService, IGrupoMusicalService grupoMusicalService, IMapper mapper)
        {
            _informativoService = informativoService;
            _pessoaService = pessoaService;
            _grupoMusicalService = grupoMusicalService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var informativos = await _informativoService.GetAll();
            return View(_mapper.Map<IEnumerable<InformativoViewModel>>(informativos));
        }

        // GET: InformativoController/Details/5
        public async Task<ActionResult> Details(uint id)
        {
            var informativo = _informativoService.Get(id);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            return View(model);
        }

        // GET: InformativoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InformativoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InformativoViewModel informativoViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _informativoService.Create(_mapper.Map<Informativo>(informativoViewModel));
                if (result == System.Net.HttpStatusCode.OK)
                    return RedirectToAction(nameof(Index));
                
            }
            return View(informativoViewModel);

        }

        // GET: InformativoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var informativo = _informativoService.Get(id);

            if(informativo == null)
            {
                Notificar("Erro! O <strong>informativo<strong/> não foi encontrado.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }
            var model = _mapper.Map<InformativoViewModel>(informativo);
            Console.WriteLine(model.Data.ToString());
            return View(model);
        }

        // POST: InformativoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InformativoViewModel informativoModel)
        {
            if (ModelState.IsValid)
            {
                Informativo informativo = _mapper.Map<Informativo>(informativoModel);
                if (_informativoService.Edit(informativo) == HttpStatusCode.OK)
                {
                    Notificar("O <b>informativo</b> foi editado com <b>Sucesso</b>!", Notifica.Sucesso);
                }
                else
                {
                    Notificar("Erro! Não foi <strong>permitido<strong/> a edição do<strong>Informativo<strong/>!", Notifica.Sucesso);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InformativoController/Delete/5
        public async Task<ActionResult> Delete(uint id)
        {
            var informativo =  _informativoService.Get(id);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            
            return View(model);
        }

        // POST: InformativoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(InformativoViewModel model)
        {
            _informativoService.Delete(model.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
