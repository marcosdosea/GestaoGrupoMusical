﻿using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using static GestaoGrupoMusicalWeb.Models.AdministradorGrupoMusicalViewModel;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class AdministradorGrupoMusicalController : Controller
    {

        private readonly IPessoaService _pessoaService;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IMapper _mapper;

        public AdministradorGrupoMusicalController(IPessoaService pessoaService,IGrupoMusicalService grupoMusicalService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _grupoMusicalService = grupoMusicalService;
            _mapper = mapper;
        }

        /// <summary>
        /// Este metodo lista todos os administradores de um determinado grupo
        /// </summary>
        /// <param name="id">id do grupo o qual queremos ver os administradores</param>
        /// <returns>lista de administradores</returns>
        public async Task<ActionResult> Index(int id)
        {
            AdministradorGrupoMusicalViewModel administradorModel = new();

            administradorModel.ListaAdministrador = await _pessoaService.GetAllAdmGroup(id);
            var grupoMusical = _grupoMusicalService.Get(id);
            if(grupoMusical == null)
            {
                return RedirectToAction(nameof(Index), "GrupoMusical");
            }
            administradorModel.Administrador.NomeGrupoMusical = grupoMusical.Nome;
            administradorModel.Administrador.IdGrupoMusical = id;

            return View(administradorModel);
        }

        /// <summary>
        /// </summary>
        /// <param name="admViewModel">viewmodel de pessoa</param>
        /// <returns></returns>
        // POST: AdministradorGrupoMusicalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdministradorModel admViewModel)
        {
            if (ModelState.IsValid)
            {
                Pessoa pessoa = new()
                {
                    Nome = admViewModel.Nome,
                    Cpf = admViewModel.Cpf,
                    Email = admViewModel.Email,
                    Sexo = admViewModel.Sexo,
                    IdGrupoMusical = admViewModel.IdGrupoMusical
                };

                _pessoaService.AddAdmGroup(pessoa);
            }
            return RedirectToAction(nameof(Index), new { id=admViewModel.IdGrupoMusical });
        }

        /// <summary>
        /// Remover o posto de administrador de grupo musical
        /// de algum usuario
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <param name="nomeGrupo">nome do grupo para por na view bag</param>
        /// <returns>view com informações do usuario para confirmar</returns>
        // GET: PessoaController/RemoveAdmGroup/5
        public ActionResult Delete(int id, string nomeGrupo)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            ViewBag.idGrupoMusical = pessoaViewModel.IdGrupoMusical;
            ViewBag.nomeGrupo = nomeGrupo;

            return View(pessoaViewModel);
        }

        /// <summary>
        /// Remove usuario do posto de adm de grupo musical
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <param name="pessoaViewModel"></param>
        /// <param name="nomeGrupo">nome do grupo para redirecionar para o index</param>
        /// <returns></returns>
        // POST: PessoaController/RemoveAdmGroup/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, int idGrupoMusical)
        {
            _pessoaService.RemoveAdmGroup(id);
            return RedirectToAction(nameof(Index), new { id= idGrupoMusical });
        }
    }
}
