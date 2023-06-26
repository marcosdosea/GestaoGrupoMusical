using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class AdministradorGrupoMusicalController : Controller
    {

        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public AdministradorGrupoMusicalController(IPessoaService pessoaService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Este metodo lista todos os administradores de um determinado grupo
        /// </summary>
        /// <param name="id">id do grupo o qual queremos ver os administradores</param>
        /// <param name="NomeGrupo">nome do grupo para por na view bag</param>
        /// <returns>lista de administradores</returns>
        public ActionResult Index(int id, string NomeGrupo)
        {
            //essas informações na viewbag serão utilizados
            //na view para poder repassar para outros metodos
            //como o create e delete
            ViewBag.Nome = NomeGrupo;
            ViewBag.IdGrupo = id;

            var listaPessoas = _pessoaService.GetAllAdmGroup(id);

            return View(listaPessoas);
        }

        // GET: AdministradorGrupoMusicalController/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// </summary>
        /// <param name="admViewModel">viewmodel de pessoa</param>
        /// <param name="nomeGrupo">nome do grupo para passar para o index</param>
        /// <returns></returns>
        // POST: AdministradorGrupoMusicalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdministradorGrupoMusicalViewModel admViewModel, string nomeGrupo)
        {
            if (ModelState.IsValid)
            {
                var pessoa = _mapper.Map<Pessoa>(admViewModel);
                _pessoaService.AddAdmGroup(pessoa);
            }
            return RedirectToAction(nameof(Index), new { id=admViewModel.IdGrupoMusical, NomeGrupo=nomeGrupo});
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
        public ActionResult Delete(int id, int idGrupoMusical, string nomeGrupo)
        {
            _pessoaService.RemoveAdmGroup(id);
            return RedirectToAction(nameof(Index), new { id= idGrupoMusical, NomeGrupo = nomeGrupo });
        }
    }
}
