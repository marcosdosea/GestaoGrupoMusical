using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class GrupoMusicalController : BaseController
    {
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IPessoaService _pessoaService;

        private readonly IMapper _mapper;
        private readonly UserManager<UsuarioIdentity> _userManager;

        public GrupoMusicalController(IGrupoMusicalService grupoMusical, IMapper mapper, UserManager<UsuarioIdentity> userManager, IPessoaService pessoaService)
        {
            _grupoMusical = grupoMusical;
            _mapper = mapper;
            _userManager = userManager;
            _pessoaService = pessoaService;
        }

        // GET: GrupoMusicalController
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult Index()
        {
            var listaGrupoMusical = _grupoMusical.GetAllDTO();
            return View(listaGrupoMusical);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO")]
        public async Task<ActionResult> IndexAdmGrupo()
        {
            var idGrupoMusical = _grupoMusical.GetIdGrupo(User.Identity.Name);

            GrupoMusicalAdmGrupoViewModel grupoMusicalViewModel = new();
            CreateColaboradorViewModel associadosToAdd = new();

            GrupoMusicalViewModel grupoMusical = _mapper.Map<GrupoMusicalViewModel>(await _grupoMusical.Get(idGrupoMusical));

            //
            grupoMusicalViewModel.GrupoMusicalViewModel = grupoMusical;

            //
            var associados = _pessoaService.GetAllPessoasOrder(idGrupoMusical);
            SelectList listAssociados = new SelectList(associados, "Id", "Nome");
            associadosToAdd.ListaAssociados = listAssociados;

            //
            var papel = await _grupoMusical.GetPapeis();
            SelectList listaPapeis = new SelectList(papel, "IdPapelGrupo", "Nome");
            associadosToAdd.ListaPapeis = listaPapeis;

            //
            grupoMusicalViewModel.ListaColaboradores = await _grupoMusical.GetAllColaboradores(idGrupoMusical);
            grupoMusicalViewModel.ListaAssociados = associadosToAdd;
            
            return View(grupoMusicalViewModel);
        }

        // GET: GrupoMusicalController/Details/5
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult Details(int id)
        {
            var grupoMusical = await _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);
            return View(grupoModel);
        }

        // GET: GrupoMusicalController/Create
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult Create()
        {
            GrupoMusicalViewModel grupoMusicalViewModel = new();
            return View(grupoMusicalViewModel);
        }

        // POST: GrupoMusicalController/Create
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var grupoModel = _mapper.Map<Grupomusical>(grupoMusicalViewModel);


                switch (await _grupoMusical.Create(grupoModel))
                {
                    case 200:

                        Notificar("Grupo <b> Cadastrado </b> com <b> Sucesso </b> ", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                        break;
                    case 500:
                        Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        return RedirectToAction(nameof(Index));
                        break;
                }

            }
            else
            {
                return View(grupoMusicalViewModel);
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: GrupoMusicalController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult Edit(int id)
        {
            var grupoMusical = await _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);

            return View(grupoModel);
        }

        // POST: GrupoMusicalController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            if (grupoMusicalViewModel.Cep != null)
                grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);

            var existe = _grupoMusical.GetCNPJExistente(id, grupoMusicalViewModel.Cnpj);
            if (existe)
            {
                ModelState.Remove("CNPJ");
            }
            if (ModelState.IsValid)
            {
                var grupoMusical = _mapper.Map<Grupomusical>(grupoMusicalViewModel);

                switch (await _grupoMusical.Edit(grupoMusical))
                {
                    case 200:
                        Notificar("Grupo Musical <b>Editado</b> com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 500:
                        Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return View(grupoMusicalViewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: GrupoMusicalController/Delete/5
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        public ActionResult Delete(int id)
        {
            var grupoMusical = await _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);
            return View(grupoModel);
        }

        // POST: GrupoMusicalController/Delete/5
        [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {

            switch (await _grupoMusical.Delete(id))
            {
                case 200:
                    Notificar("Grupo <b> Excluido </b> com <b> Sucesso </b> ", Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                    break;
                case 500:
                    Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante a <b>Exclusão</b> do Gruppo Musical, se isso persistir entre em contato com o suporte", Notifica.Erro);
                    return RedirectToAction(nameof(Index));
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
