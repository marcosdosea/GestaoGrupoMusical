using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using static GestaoGrupoMusicalWeb.Models.AdministradorGrupoMusicalViewModel;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
    public class AdministradorGrupoMusicalController : BaseController
    {

        private readonly IPessoaService _pessoaService;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IMapper _mapper;

        private readonly UserManager<UsuarioIdentity> _userManager;

        public AdministradorGrupoMusicalController(IPessoaService pessoaService,IGrupoMusicalService grupoMusicalService,
                                                   IMapper mapper, UserManager<UsuarioIdentity> userManager)
        {
            _pessoaService = pessoaService;
            _grupoMusicalService = grupoMusicalService;
            _mapper = mapper;
            _userManager = userManager;
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
        public async Task<ActionResult> Create(AdministradorModel admViewModel)
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

                await _pessoaService.AddAdmGroup(pessoa);

                switch (await RequestPasswordReset(_userManager, pessoa.Email))
                {
                    case 200:
                        Notificar("<b>Sucesso!</b> Administrador cadastrado e email para redefinição enviado.", Notifica.Sucesso); break;
                    default:
                        Notificar("<b>Erro!</b> Não foi possível enviar o email para redefinição de senha.", Notifica.Erro); break;
                }
            }
            return RedirectToAction(nameof(Index), new { id=admViewModel.IdGrupoMusical });
        }

        /// <summary>
        /// Remover o posto de administrador de grupo musical
        /// de algum usuario
        /// </summary>
        /// <param name="id">id do usuario alvo</param>
        /// <returns>view com informações do usuario para confirmar</returns>
        // GET: PessoaController/RemoveAdmGroup/5
        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            ViewBag.idGrupoMusical = pessoaViewModel.IdGrupoMusical;

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
        public async Task<ActionResult> Delete(int id, int idGrupoMusical)
        {
            await _pessoaService.RemoveAdmGroup(id);
            return RedirectToAction(nameof(Index), new { id= idGrupoMusical });
        }

        public async Task<ActionResult> Notificar(int id)
        {
            var pessoa = _pessoaService.Get(id);
            if (pessoa.IdPapelGrupo == 3)
            {
                await _pessoaService.NotificarCadastroAdmGrupoAsync(pessoa);
            }
            return RedirectToAction(nameof(Index), new { id = pessoa.IdGrupoMusical });
        }
    }
}
