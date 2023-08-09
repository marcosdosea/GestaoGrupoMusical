﻿using AutoMapper;
using Core;
using Core.DTO;
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
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IManequimService _manequim;
        private readonly IFigurinoService _figurinoService;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public FigurinoController(IMapper mapper, IGrupoMusicalService grupoMusical,
            IManequimService manequim, IFigurinoService figurino, UserManager<UsuarioIdentity> userManager)
        {
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _manequim = manequim;
            _figurinoService = figurino;
            _userManager = userManager;
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
                figurinoViewModel.IdGrupoMusical = _grupoMusical.GetIdGrupo(User.Identity.Name);

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

                figurino.IdGrupoMusical = _grupoMusical.GetIdGrupo(User.Identity.Name);

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
        public async Task<ActionResult> Estoque(int id)
        {
            EstoqueDTOViewModel estoqueDTOviewModel = new();
            var figurino = await _figurinoService.Get(id);

            var estoque = await _figurinoService.GetAllEstoqueDTO(id);

            estoqueDTOviewModel.TabelaEstoques = estoque;

            estoqueDTOviewModel.Nome = figurino.Nome;
            estoqueDTOviewModel.Data = figurino.Data;

            return View(estoqueDTOviewModel);
        }

        public async Task<ActionResult> CreateEstoque(int idFigurino)
        {
            var figurino = await _figurinoService.Get(idFigurino);
            var manequins = _manequim.GetAll();

            SelectList listManequins = new SelectList(manequins, "Id", "Tamanho");

            CreateEstoqueViewModel estoqueViewModel = new()
            {
                IdFigurino = figurino.Id,
                Data = figurino.Data.Value.ToString("dd/MM/yyyy"),
                listManequim = listManequins

            };

            return View(estoqueViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEstoque(CreateEstoqueViewModel estoqueDTO)
        {
            /*Figurinomanequim estoque = new()
            {
                IdFigurino = estoqueDTO.IdFigurino,
                IdManequim = estoqueDTO.IdManequim,
                QuantidadeDisponivel = estoqueDTO.Disponivel,
                QuantidadeEntregue = estoqueDTO.Entregues
            };

            int resul = await _figurinoService.CreateEstoque(estoque);

            switch (resul)
            {
                case 200:
                    Notificar("<b>Sucesso</b>! Estoque cadastrado.", Notifica.Sucesso);
                    break;
                case 400:
                    Notificar("<b>Alerta</b>! Dados insuficientes.", Notifica.Alerta);
                    break;
                case 401:
                    Notificar("<b>Alerta</b>! Sem quantidade disponível para estoque.", Notifica.Alerta);
                    break;
                case 500:
                    Notificar("<b>Erro</b>! Algum problema ao registrar estoque.", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Erro</b>! Algum problema ao tentar registrar estoque.", Notifica.Erro);
                    break;
            }*/
            return View();
        }
    }
}
