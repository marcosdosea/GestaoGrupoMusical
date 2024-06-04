using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NuGet.Protocol;
using Service;
using System.Collections;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class MaterialEstudoController : BaseController
    {
        private readonly IMaterialEstudoService _materialEstudo;
        private readonly IMapper _mapper;

        public MaterialEstudoController(IMaterialEstudoService materialEstudo, IMapper mapper)
        {
            _materialEstudo = materialEstudo;
            _mapper = mapper;
        }


        public static void Teste()
        {
            Console.WriteLine("\n######################");
        }
        // GET: MaterialEstudoController
        public async Task<ActionResult> Index()
        {
            var listaMaterialEstudo = await _materialEstudo.GetAll();
            var listaMaterialEstudoModel = _mapper.Map<List<MaterialEstudoViewModel>>(listaMaterialEstudo);
            return View(listaMaterialEstudoModel);
        }

        // GET: MaterialEstudoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var listaMaterialEstudo = await _materialEstudo.Get(id);
            if (listaMaterialEstudo != null)
            {
                var listaMaterialEstudoModel = _mapper.Map<List<Materialestudo>>(listaMaterialEstudo);
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
            var model = _mapper.Map<Materialestudo>(materialEstudo);
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
            var materialEstudo = await _materialEstudo.Get(id);
            var model = _mapper.Map<MaterialEstudoViewModel>(materialEstudo);

            return View(model);
        }

        // POST: MaterialEstudoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, MaterialEstudoViewModel materialEstudo)
        {
            var material = _materialEstudo.Get(id);
            if(material != null)
            {
                await _materialEstudo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            return View(materialEstudo);
        }
    }
}
