﻿using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR GRUPO")]
    public class PessoaController : BaseController
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IManequimService _manequim;

        private readonly UserManager<UsuarioIdentity> _userManager;

        public PessoaController (IPessoaService pessoaService, IMapper mapper, IGrupoMusicalService grupoMusical, IManequimService manequim, UserManager<UsuarioIdentity> userManager)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
            _grupoMusical = grupoMusical;
            _manequim = manequim;
            _userManager = userManager;
        }

        // GET: PessoaController
        public async Task<IActionResult> Index(int? page)
        {
            var numeroPagina = page ?? 1; // se pagina null retorna 1;
            int qtdItem = 10;
            var paginaPessoa = await _pessoaService.GetAllAssociadoDTOByGroup(User.Identity.Name);
            
            return View(paginaPessoa.ToPagedList(numeroPagina, qtdItem));
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

            IEnumerable<Papelgrupo> listaPapelGrupo = _pessoaService.GetAllPapelGrupo();
            IEnumerable<Grupomusical> listaGrupoMusical = _grupoMusical.GetAll();
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);

            return View(pessoaViewModel);
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PessoaViewModel pessoaViewModel)
        {
            String mensagem = String.Empty;
            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            if (await _pessoaService.AssociadoExist(pessoaViewModel.Email))
            {
                mensagem = "<b>Alerta!</b> Email já está em uso";
                Notificar(mensagem, Notifica.Alerta);
                pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);
                return View("Create", pessoaViewModel);
            }

            if (ModelState.IsValid)
            {
                var colaborador = await _pessoaService.GetByCpf(User.Identity?.Name);
                if (colaborador == null)
                {
                    return RedirectToAction("Sair", "Identity");
                }
                var pessoaModel = _mapper.Map<Pessoa>(pessoaViewModel);
                pessoaModel.IdPapelGrupo = 1;
                pessoaModel.IdGrupoMusical = colaborador.IdGrupoMusical;

                switch (await _pessoaService.AddAssociadoAsync(pessoaModel))
                {
                    case 200:
                        switch (await RequestPasswordReset(_userManager, pessoaModel.Email, pessoaModel.Nome))
                        {
                            case 200:
                                mensagem = "<b>Sucesso</b>! Associado cadastrado e enviado email para redefinição de senha.";
                                Notificar(mensagem, Notifica.Sucesso);
                                break;
                            default:
                                mensagem = "<b>Alerta</b>! Associado cadastrado, mas não foi possível enviar o email para redefinição de senha.";
                                Notificar(mensagem, Notifica.Alerta);
                                break;
                        }
                        return RedirectToAction(nameof(Index));

                    case 500:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte";
                        Notificar(mensagem , Notifica.Erro);
                        break;

                    case 400:
                        mensagem = "<b>Alerta</b> ! Não foi possível cadastrar, a data de entrada deve ser menor que " + DateTime.Now.ToShortDateString();
                        Notificar(mensagem, Notifica.Alerta);
                        break;

                    case 401:
                        mensagem = "<b>Alerta</b> ! Não foi possível cadastrar, a data de nascimento deve ser menor que " + DateTime.Now.ToShortDateString() + " e menor que 120 anos ";
                        Notificar(mensagem, Notifica.Alerta);
                        break;

                    case 450:
                        mensagem = "Ocorreu um <b>Erro</b> durante a liberação de acesso ao <b>Associado</b>, se isso persistir entre em contato com o suporte";
                        Notificar(mensagem, Notifica.Erro);
                        break;

                    default:
                        mensagem = "Ocorreu um <b>Erro</b> durante o cadastro.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                }

            }

            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", null);
            return View("Create", pessoaViewModel);
        }

        // GET: PessoaController/Edit/5
        public ActionResult Edit(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            IEnumerable<Manequim> listaManequim = _manequim.GetAll();

            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);



            return View(pessoaViewModel);
        }

        // POST: PessoaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PessoaViewModel pessoaViewModel)
        {
            var cpf = _pessoaService.GetCPFExistente(id,pessoaViewModel.Cpf);

            if (cpf)
            {
                ModelState.Remove("Cpf");
            }

            if (ModelState.IsValid)
            {
                var pessoa = _mapper.Map<Pessoa>(pessoaViewModel);
                String mensagem = String.Empty;
                switch (await _pessoaService.Edit(pessoa))
                {
                    case 200:
                        Notificar("Associado <b>Editado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 500:
                        Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Editar</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        break;
                    case 400:
                        mensagem = "<b>Alerta</b> ! Não foi possível editar, a data de entrada deve ser menor que " + DateTime.Now.ToShortDateString();
                        Notificar(mensagem, Notifica.Alerta);
                        break;
                    case 401:
                        mensagem = "<b>Alerta</b> ! Não foi possível editar, a data de nascimento deve ser menor que " + DateTime.Now.ToShortDateString() + " e menor que 120 anos ";
                        Notificar(mensagem, Notifica.Alerta);
                        break;

                }
            }

            IEnumerable<Manequim> listaManequim = _manequim.GetAll();
            pessoaViewModel.ListaManequim = new SelectList(listaManequim, "Id", "Tamanho", pessoaViewModel.IdManequim);
            return View("Edit", pessoaViewModel);
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
            return RedirectToAction(nameof(Index));
        }

        public ActionResult RemoveAssociado(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);
            return View(pessoaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAssociado(int id, PessoaViewModel pessoaViewModel)
        {
            var pessoassociada = _pessoaService.Get(id);
            String mensagem = String.Empty;
            switch (await _pessoaService.RemoverAssociado(pessoassociada, pessoaViewModel.MotivoSaida)){
                case 200:
                    mensagem = "Associado <b>Excluído</b> com <b>Sucesso</b>";
                    Notificar(mensagem, Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                case 500:
                    mensagem = "<b>Erro</b> ! erro ao <b>Excluir</b> um associado, se isso persistir entre em contato com o suporte";
                    Notificar(mensagem, Notifica.Erro);
                    return RedirectToAction("Delete", pessoassociada);

            }
            return RedirectToAction(nameof(Index));
        }

    }
}
