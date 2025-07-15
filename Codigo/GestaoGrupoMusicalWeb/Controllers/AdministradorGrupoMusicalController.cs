using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System.Net;
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

        private readonly UserManager<UsuarioIdentity> _userManager;

        public AdministradorGrupoMusicalController(
            IPessoaService pessoaService, 
            IGrupoMusicalService grupoMusicalService,                                      
            IMapper mapper, 
            UserManager<UsuarioIdentity> userManager,
            ILogger<BaseController> logger)
                : base(logger)
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
            var grupoMusical = await _grupoMusicalService.Get(id);
            if (grupoMusical == null)
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
                String mensagem = String.Empty;

                if (await _pessoaService.AssociadoExist(admViewModel.Email))
                {
                    mensagem = "<b>Alerta!</b> Email já está em uso";
                    Notificar(mensagem, Notifica.Alerta);

                    return RedirectToAction(nameof(Index));
                }

                HttpStatusCode retAddAdm = await _pessoaService.AddAdmGroup(pessoa);

                switch (retAddAdm)
                {
                    case HttpStatusCode.Created:
                        switch (await RequestPasswordReset(_userManager, pessoa.Email, pessoa.Nome))
                        {
                            case HttpStatusCode.OK:
                                mensagem = "<b>Sucesso</b>! Administrador cadastrado e enviado email para redefinição de senha.";
                                Notificar(mensagem, Notifica.Sucesso);
                                break;
                            default:
                                mensagem = "<b>Alerta</b>! Administrador cadastrado, mas não foi possível enviar o email para redefinição de senha.";
                                Notificar(mensagem, Notifica.Alerta);
                                break;
                        }
                        break;

                    case HttpStatusCode.OK:
                        mensagem = "<b>Sucesso</b>! Associado promovido a <b>Administrador do Grupo Musical</b>.";
                        Notificar(mensagem, Notifica.Sucesso);
                        break;

                    case HttpStatusCode.NotAcceptable:
                        mensagem = "<b>Alerta</b>! Infelizemente não foi possível <b>cadastrar</b>, o usuário faz parte de outro grupo musical";
                        Notificar(mensagem, Notifica.Alerta);
                        break;

                    case HttpStatusCode.BadRequest:
                        mensagem = "<b>Erro</b>! Associado já é um administrador deste grupo.";
                        Notificar(mensagem, Notifica.Erro);
                        break;

                    case HttpStatusCode.NotImplemented:
                        mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro durante a <b>Promoção</b> do associado para administrador.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                    case HttpStatusCode.InternalServerError:
                        mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro durante a <b>Operação</b>.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                }

            }
            return RedirectToAction(nameof(Index), new { id = admViewModel.IdGrupoMusical });
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
            HttpStatusCode result = await _pessoaService.RemoveAdmGroup(id);
            String mensagem = String.Empty;
            switch (result)
            {
                case HttpStatusCode.OK:
                    mensagem = "<b>Sucesso</b>! Administrador removido.";
                    Notificar(mensagem, Notifica.Sucesso);
                    break;

                case HttpStatusCode.NotFound:
                    mensagem = "<b>Erro</b>! Administrador não encontrado.";
                    Notificar(mensagem, Notifica.Erro);
                    break;
                case HttpStatusCode.NotAcceptable:
                    mensagem = "<b>Alerta</b>! Não é possível <b>remover</b> o único adminstrador do grupo.";
                    Notificar(mensagem, Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    mensagem = "<b>Erro</b>! Desculpe, ocorreu um erro durante a <b>Operação</b>.";
                    Notificar(mensagem, Notifica.Erro);
                    break;

            }
            return RedirectToAction(nameof(Index), new { id = idGrupoMusical });
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
