using AutoMapper;
using Core;
using Core.Datatables;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class MaterialEstudoController : BaseController
    {
        private readonly IMaterialEstudoService _materialEstudo;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IMapper _mapper;

        public MaterialEstudoController(IMaterialEstudoService materialEstudo,IGrupoMusicalService grupoMusical, IMapper mapper)
        {
            _materialEstudo = materialEstudo;
            _grupoMusical = grupoMusical;
            _mapper = mapper;
        }
        // GET: MaterialEstudoController
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Index()
        {
            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);
            var listaMaterialEstudo = await _materialEstudo.GetAllMaterialEstudoPerIdGrupo(idGrupoMusical);
            var listaMaterialEstudoModel = _mapper.Map<List<MaterialEstudoViewModel>>(listaMaterialEstudo);
            return View(listaMaterialEstudoModel);
        }

        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var response = await _materialEstudo.GetDataPage(request, await _grupoMusical.GetIdGrupo(User.Identity.Name));
            return Json(response);
        }

        [HttpPost]
        public async Task<ActionResult> Details(int id)
        {
            var listaMaterialEstudo = await _materialEstudo.Get(id);
            if (listaMaterialEstudo != null)
            {
                var listaMaterialEstudoModel = _mapper.Map<List<MaterialEstudoViewModel>>(listaMaterialEstudo);
                return View(listaMaterialEstudoModel);
            }
            Notificar("<b>Material de estudo não encontrado!</b>", Notifica.Alerta);
            return RedirectToAction(nameof(Index));
        }

        // GET: MaterialEstudoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MaterialEstudoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MaterialEstudoViewModel materialEstudoViewlModel)
        {
            if(ModelState.IsValid)
            {
                if(await _materialEstudo.Create(_mapper.Map<Materialestudo>(materialEstudoViewlModel)))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(materialEstudoViewlModel);
        }

        // GET: MaterialEstudoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var materialEstudo = await _materialEstudo.Get(id);
            var model = _mapper.Map<MaterialEstudoViewModel>(materialEstudo);
            return View(model);
        }

        // POST: MaterialEstudoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, MaterialEstudoViewModel materialEstudo)
        {
            if (ModelState.IsValid)
            {
                if (await _materialEstudo.Edit(_mapper.Map<Materialestudo>(materialEstudo)))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(materialEstudo);
        }

        // GET: MaterialEstudoController/Delete/5
        public async Task<ActionResult>Delete(int id)
        {
            Console.WriteLine("\n############## PRIMEIRO ##############");
            var materialEstudo = await _materialEstudo.Get(id);
            var model = _mapper.Map<MaterialEstudoViewModel>(materialEstudo);
            Notificar("Não é permitido <b>Deletar</b> os dados de um instrumento <b>Emprestado</b>.", Notifica.Alerta);
            return RedirectToAction(nameof(Index));
        }

        // POST: MaterialEstudoController/Delete/5

        public async Task<ActionResult> Delete(int id, MaterialEstudoViewModel instrumentoMusicalViewModel)
        {
            Console.WriteLine("\n############## SEGUNDO ##############");
            switch (await _materialEstudo.Delete(id))
            {
                case HttpStatusCode.OK:
                    Notificar("Material de Estudo <b>Excluído</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                default :
                    Notificar($"Nenhum <b>Material de Estudo</b> foi encontrado <b>{id}</b>.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
