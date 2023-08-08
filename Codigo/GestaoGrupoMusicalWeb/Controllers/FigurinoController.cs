﻿using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Service;
using System.Data;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR GRUPO")]
    public class FigurinoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IManequimService _manequimService;
        private readonly IFigurinoService _figurinoService;
        private readonly IPessoaService _pessoaService;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public FigurinoController(IMapper mapper, IGrupoMusicalService grupoMusical,
            IManequimService manequim, IFigurinoService figurino,UserManager<UsuarioIdentity> userManager, IPessoaService pessoa)
        {
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _manequimService = manequim;
            _figurinoService = figurino;
            _userManager = userManager;
            _pessoaService = pessoa;
        }

        // GET: FigurinoController
        public async Task<ActionResult> Index()
        {
            var listFigurinos = await _figurinoService.GetAll(User.Identity.Name);

            var listFigurinosViewModdel = _mapper.Map<IEnumerable<FigurinoViewModel>>(listFigurinos);

            return View(listFigurinosViewModdel);
        }

        // GET: FigurinoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FigurinoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FigurinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FigurinoViewModel figurinoViewModel)
        {
            if (ModelState.IsValid)
            {
                figurinoViewModel.IdGrupoMusical = _grupoMusicalService.GetIdGrupo(User.Identity.Name);

                var figurino = _mapper.Map<Figurino>(figurinoViewModel);
                int resul = await _figurinoService.Create(figurino);


                switch (resul)
                {
                    case 200:
                        Notificar("<b>Sucesso</b>! Figurino cadastrado!", Notifica.Sucesso);
                        break;
                    case 500:
                        Notificar("<b>Erro</b>! Algo deu errado ao cadastrar novo figurino", Notifica.Erro);
                        return View(figurinoViewModel);
                    default:
                        Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                        break;
                }

                return RedirectToAction("Index");
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                return View();
            }
        }

        // GET: FigurinoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var figurino = await _figurinoService.Get(id);
            var figurinoViewModel = _mapper.Map<FigurinoViewModel>(figurino);

            return View(figurinoViewModel);
        }

        // POST: FigurinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, FigurinoViewModel figurinoViewModel)
        {
            if (ModelState.IsValid)
            {
                var figurino = _mapper.Map<Figurino>(figurinoViewModel);

                figurino.IdGrupoMusical = _grupoMusicalService.GetIdGrupo(User.Identity.Name);

                int resul = await _figurinoService.Edit(figurino);
                if (resul == 200)
                {
                    Notificar("<b>Sucesso</b>! Figurino alterado!", Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                    return View(figurinoViewModel);
                } 
            }
            else
            {
                Notificar("<b>Erro</b>! Há algo errado com os dados", Notifica.Erro);
                return View();
            }
        }

        // GET: FigurinoController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var figurino = await _figurinoService.Get(id);
            var figurinoViewModel = _mapper.Map<FigurinoViewModel>(figurino);

            return View(figurinoViewModel);
        }

        // POST: FigurinoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, FigurinoViewModel figurinoViewModel)
        {
            int resul = await _figurinoService.Delete(id);

            if (resul == 200)
            {
                Notificar("<b>Sucesso</b>! Figurino removido!", Notifica.Sucesso);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Notificar("<b>Erro</b>! Algo deu errado ao remover figurino. Verifique se há estoque emprestado.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> Movimentar(int id)
        {
            var figurino = await _figurinoService.Get(id);
            var manequins = _manequimService.GetAll();

            int idGrupo = _grupoMusicalService.GetIdGrupo(User.Identity.Name);
            var associados = _pessoaService.GetAllPessoasOrder(idGrupo);


            SelectList listAssociados = new SelectList(associados, "Id", "Nome", associados.First().Id );
            SelectList listManequins = new SelectList(manequins, "Id", "Tamanho", manequins.First().Id );

            var movimentarFigurinoViewModel = new MovimentacaoFigurinoViewModel
            {
                IdFigurino = figurino.Id,
                NomeFigurino = figurino.Nome,
                DataFigurinoString = figurino.Data.Value.ToString("dd/MM/yyyy"),
                ListaAssociado = listAssociados,
                ListaManequim = listManequins
            };

            return View(movimentarFigurinoViewModel);
        }
    }
}
