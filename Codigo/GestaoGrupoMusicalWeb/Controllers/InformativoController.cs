using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InformativoController : Controller
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
        public async Task<ActionResult> Details(int idPessoa, int idGrupoMusical)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            return View(model);
        }

        // GET: InformativoController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataPage()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var sortColumn = HttpContext.Request.Form["order[0][column]"].FirstOrDefault();
            var sortColumnDir = HttpContext.Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var informativos = await _informativoService.GetAll();
            var informativoData = informativos.AsQueryable();


            var recordsTotal = informativoData.Count();

            var data = informativoData.Skip(skip).Take(pageSize).ToList();

            var result = new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = _mapper.Map<IEnumerable<InformativoViewModel>>(data)
            };

            return Ok(result);
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
        public async Task<ActionResult> Edit(int idGrupoMusical, int idPessoa)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);

            return View(model);
        }

        // POST: InformativoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InformativoViewModel informativo)
        {
            if (ModelState.IsValid)
            {
                if(await _informativoService.Edit(_mapper.Map<Informativo>(informativo)))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(informativo);

        }

        // GET: InformativoController/Delete/5
        public async Task<ActionResult> Delete(int idGrupoMusical, int idPessoa)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            
            return View(model);
        }

        // POST: InformativoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int idGrupoMusical,int idPessoa, InformativoViewModel informativo)
        {
            await _informativoService.Delete(idGrupoMusical, idPessoa);
            return RedirectToAction(nameof(Index));
        }

    }
}
