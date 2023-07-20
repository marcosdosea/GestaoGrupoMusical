using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using static GestaoGrupoMusicalWeb.Controllers.BaseController;
using static GestaoGrupoMusicalWeb.Models.AdministradorGrupoMusicalViewModel;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR SISTEMA")]
    public class AdministradorGrupoMusicalController : BaseController
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
                String mensagem = String .Empty;
                switch(await _pessoaService.AddAdmGroup(pessoa))
                {
                    case 200:
                        mensagem = "Administrador do grupo musical <b>Cadastrado</b> com <b>Sucesso</b>";
                        Notificar(mensagem, Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case 400:
                        mensagem = "<b>Alerta</b> ! Infelizemente não foi possível <b>cadastrar</b>, o usuário faz parte de outro grupo musical";
                        Notificar(mensagem, Notifica.Alerta);
                        return RedirectToAction("Index", admViewModel);
                    case 500:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do administrador do grupo musical, se isso persistir entre em contato com o suporte";
                        Notificar(mensagem, Notifica.Erro);
                        return RedirectToAction("Index", admViewModel);

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
