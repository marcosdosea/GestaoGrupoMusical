using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public PessoaController (IPessoaService pessoaService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
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
            return View();
        }

        // POST: PessoaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PessoaViewModel pessoaViewModel)
        {
            if (ModelState.IsValid)
            {
                var pessoa = _mapper.Map<Pessoa>(pessoaViewModel);
                _pessoaService.Create(pessoa);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: PessoaController/Edit/5
        public ActionResult Edit(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

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

        //Administradores do Grupo Musical\\
        /// <summary>
        /// Este metodo lista todos os administradores de um determinado grupo
        /// </summary>
        /// <param name="id">id do grupo o qual queremos ver os administradores</param>
        /// <returns>lista de administradores</returns>
        public ActionResult IndexAdmGroup(int id)
        {
            var listaPessoas = _pessoaService.GetAllAdmGroup(id);
            var listaPessoasModel = _mapper.Map<List<PessoaViewModel>>(listaPessoas);

            return View(listaPessoasModel);
        }

        /// <summary>
        /// Remover o posto de administrador de grupo musical
        /// de algum usuario
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <returns>view com informações do usuario para confirmar</returns>
        // GET: PessoaController/RemoveAdmGroup/5
        public ActionResult RemoveAdmGroup(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        /// <summary>
        /// Remove usuario do posto de adm de grupo musical
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <param name="pessoaViewModel"></param>
        /// <returns></returns>
        // POST: PessoaController/RemoveAdmGroup/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAdmGroup(int id, PessoaViewModel pessoaViewModel)
        {
            _pessoaService.RemoveAdmGroup(id);
            return RedirectToAction(nameof(IndexAdmGroup));
        }
    }
}
