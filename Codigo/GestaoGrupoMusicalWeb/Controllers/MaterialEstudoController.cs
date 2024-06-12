﻿using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class MaterialEstudoController : BaseController
    {
        private readonly IMaterialEstudoService _materialEstudo;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IMapper _mapper;
        private readonly IPessoaService _pessoa;

        private readonly IMaterialEstudoService _materialEstudoService;

        // Injeção de dependência através do construtor
        public MaterialEstudoController(IMaterialEstudoService materialEstudoService, IPessoaService pessoa, IMaterialEstudoService materialEstudo, IGrupoMusicalService grupoMusical, IMapper mapper)
        {
            _materialEstudoService = materialEstudoService;
            _materialEstudo = materialEstudo;
            _grupoMusical = grupoMusical;
            _mapper = mapper;
            _pessoa = pessoa;
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
        public async Task<ActionResult> Create(MaterialEstudoViewModel materialEstudoViewModel)
        {
            if (ModelState.IsValid)
            {
                UserDTO user = await _pessoa.GetByCpf(User.Identity.Name);

                var materialEstudo = new Materialestudo
                {
                    Nome = materialEstudoViewModel.Nome,
                    Link = materialEstudoViewModel.Link,
                    Data = materialEstudoViewModel.Data,
                    IdGrupoMusical = user!.IdGrupoMusical,
                    IdColaborador = user.Id,
                };

                var statusCode = await _materialEstudoService.Create(materialEstudo);
                if (statusCode == HttpStatusCode.Created)
                {
                    Notificar("Material de Estudo <b>Cadastrado</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                }
                Notificar("<b>Erro</b>! Há algo errado ao cadastrar Material de Estudo", Notifica.Erro);
                return RedirectToAction("Index");
            }
            else
            {
                Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                return View();
            }

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
        public async Task<ActionResult> Delete(int id)
        {
            var materialEstudo = await _materialEstudo.Get(id);
            var model = _mapper.Map<MaterialEstudoViewModel>(materialEstudo);

            return View(model);
        }

        // POST: MaterialEstudoController/Delete/5

        public async Task<ActionResult> Delete(int id, MaterialEstudoViewModel instrumentoMusicalViewModel)
        {
            switch (await _materialEstudo.Delete(id))
            {
                case HttpStatusCode.OK:
                    Notificar("Material de Estudo <b>Excluído</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar($"Nenhum <b>Material de Estudo</b> foi encontrado <b>{id}</b>.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
