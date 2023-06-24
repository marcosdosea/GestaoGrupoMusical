﻿using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IPapelGrupoService _papelGrupo;
        private readonly IManequimService _manequim;

        public PessoaController (IPessoaService pessoaService, IMapper mapper, IGrupoMusicalService grupoMusical, IPapelGrupoService papelgrupo, IManequimService manequim)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _papelGrupo = papelgrupo;
            _manequim = manequim;
        }

        // GET: PessoaController
        public ActionResult Index()
        {
            var listaPessoas = _pessoaService.GetAll();
            var listaPessoasModel = _mapper.Map<List<PessoaViewModel>>(listaPessoas);

            return View(listaPessoasModel);
        }

        // GET: PessoaController/Details/5
        public ActionResult Details(int id)
        {
            var pessoa = _pessoaService.Get(id);
            PessoaViewModel pessoaModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaModel);
        }

        // GET: PessoaController/Create
        public ActionResult Create()
        {
            PessoaViewModel pessoaViewModel = new PessoaViewModel();

            IEnumerable<Papelgrupo> listaPapelGrupo = _papelGrupo.GetAll();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", null);
            pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", null);
            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PessoaViewModel pessoaViewModel)
        {
            if (ModelState.IsValid)
            {
                var pessoaModel = _mapper.Map<Pessoa>(pessoaViewModel);
                await _pessoaService.Create(pessoaModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Edit/5
        public ActionResult Edit(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            IEnumerable<Papelgrupo> listaPapelGrupo = _papelGrupo.GetAll();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaGrupoMusical = new SelectList(listaGrupoMusical, "Id", "Nome", pessoaViewModel.IdGrupoMusical);
            pessoaViewModel.ListaPapelGrupo = new SelectList(listaPapelGrupo, "IdPapelGrupo", "Nome", pessoaViewModel.IdPapelGrupo);
            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);



            return View(pessoaViewModel);
        }

        // POST: PessoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PessoaViewModel pessoaViewModel)
        {
            if (ModelState.IsValid)
            {
                var pessoa = _mapper.Map<Pessoa>(pessoaViewModel);
                _pessoaService.Edit(pessoa);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Delete/5
        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, PessoaViewModel pessoaViewModel)
        {
            _pessoaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Esse metodo tem a função de trocar um usuario
        /// para colaborador
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <returns>retorna a viewmodel da pessoa encontrada</returns>
        // GET: PessoaController/ChangeToCollaborator/5
        public ActionResult ChangeToCollaborator(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/ChangeToCollaborator/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeToCollaborator(int id, PessoaViewModel pessoaViewModel)
        {

            pessoaViewModel.IdPapelGrupo = 2;

            var pessoa = _mapper.Map<Pessoa>(pessoaViewModel);

            _pessoaService.Edit(pessoa);
            return RedirectToAction(nameof(Index));
        }
    }
}
