using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class EnsaioController : BaseController
    {
        private readonly IEnsaioService _ensaio;
        private readonly IMapper _mapper;
        private readonly IPessoaService _pessoa;
        private readonly IGrupoMusicalService _grupoMusical;

        public EnsaioController(IMapper mapper, IEnsaioService ensaio, IPessoaService pessoa, IGrupoMusicalService grupoMusical)
        {
            _ensaio = ensaio;
            _mapper = mapper;
            _pessoa = pessoa;
            _grupoMusical = grupoMusical;
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController
        public async Task<ActionResult> Index()
        {
            int idGrupo = Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value);
            var ensaios = await _ensaio.GetAllIndexDTO(idGrupo);
            return View(ensaios);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var ensaio = _ensaio.GetDetailsDTO(id);
            if (Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value) != ensaio.IdGrupoMusical)
            {
                Notificar("<b>Ensaio não encontrado!</b>", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
            return View(ensaio);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/Create
        public async Task<ActionResult> Create()
        {
            var lista = await _pessoa.GetRegentesForAutoCompleteAsync(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));

            if(lista == null || !lista.Any())
            {
                Notificar("É necessário cadastrar um Regente para então cadastrar um Ensaio.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            EnsaioViewModel ensaioModel = new();
            
            ensaioModel.ListaPessoa = new SelectList(lista, "Id", "Nome");

            ViewData["exemploRegente"] = lista.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            ensaioModel.JsonLista = lista.ToJson();
            return View(ensaioModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // POST: EnsaioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EnsaioViewModel ensaioViewModel)
        {
            if (ModelState.IsValid && ensaioViewModel.IdRegentes != null)
            {
                String mensagem = String.Empty;
                var ensaio = _mapper.Map<Ensaio>(ensaioViewModel);
              
                ensaio.IdGrupoMusical = Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value);
                ensaio.IdColaboradorResponsavel = Convert.ToInt32(User.FindFirst("Id")?.Value);
                switch (await _ensaio.Create(ensaio, ensaioViewModel.IdRegentes))
                {
                    case HttpStatusCode.OK:
                        mensagem = "Ensaio <b>Cadastrado</b> com <b>Sucesso</b>";
                        Notificar(mensagem, Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case HttpStatusCode.BadRequest:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser menor que a data de <b>fim</b>";
                        Notificar(mensagem, Notifica.Alerta);
                        break;
                    case HttpStatusCode.PreconditionFailed:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser maior que a data de hoje " + DateTime.Now;
                        Notificar(mensagem, Notifica.Alerta);
                        break;
                    case HttpStatusCode.InternalServerError:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> de ensaio.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                }
            }
            var lista = await _pessoa.GetRegentesForAutoCompleteAsync(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));
            ensaioViewModel.ListaPessoa = new SelectList(lista, "Id", "Nome");

            ensaioViewModel.JsonLista = lista.ToJson();
            return View(ensaioViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ensaio = await _ensaio.Get(id);
            if (Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value) != ensaio.IdGrupoMusical)
            {
                Notificar("<b>Ensaio não encontrado!</b>", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
            EnsaioViewModel ensaioModel = _mapper.Map<EnsaioViewModel>(ensaio);

            ensaioModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");

            return View(ensaioModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // POST: EnsaioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnsaioViewModel ensaioViewModel)
        {
            if (ModelState.IsValid)
            {
                String mensagem = String.Empty;
                switch (await _ensaio.Edit(_mapper.Map<Ensaio>(ensaioViewModel)))
                {
                    case HttpStatusCode.OK:
                        mensagem = "Ensaio <b>Editado</b> com <b>Sucesso</b>";
                        Notificar(mensagem, Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case HttpStatusCode.PreconditionFailed:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser menor que a data de <b>a data de fim</b>, ou a <b>a data de fim</b> tem que ser maior";
                        Notificar(mensagem, Notifica.Alerta);
                        break;
                    case HttpStatusCode.BadRequest:
                        mensagem = "Alerta ! A <b>data de início</b> deve ser maior que a data de hoje " + DateTime.Now;
                        Notificar(mensagem, Notifica.Alerta);
                        break;
                    case HttpStatusCode.InternalServerError:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Editar</b> de ensaio.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                }
            }
            ensaioViewModel.ListaPessoa = new SelectList(_pessoa.GetAll(), "Id", "Nome");
            return View(ensaioViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var ensaio = await _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // POST: EnsaioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(EnsaioViewModel ensaioModel)
        {
            String mensagem = String.Empty;
            switch (await _ensaio.Delete(ensaioModel.Id))
            {
                case HttpStatusCode.OK:
                    mensagem = "Ensaio <b>Deletado</b> com <b>Sucesso</b>";
                    Notificar(mensagem, Notifica.Sucesso);
                    break;
                case HttpStatusCode.InternalServerError:
                    mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante ao <b>Excluir</b> um ensaio.";
                    Notificar(mensagem, Notifica.Erro);
                    break;

            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        public async Task<ActionResult> RegistrarFrequencia(int idEnsaio)
        {
            if (User.FindFirst("IdGrupoMusical")?.Value == null) {
                return RedirectToAction("Sair", "Identity");
            }
            var frequencias = await _ensaio.GetFrequenciaAsync(idEnsaio, Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));
            if(frequencias == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(frequencias);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrarFrequencia(List<EnsaioListaFrequenciaDTO> listaFrequencia)
        {
            switch(await _ensaio.RegistrarFrequenciaAsync(listaFrequencia))
            {
                case HttpStatusCode.OK:
                    Notificar("Lista de <b>Frequência</b> salva com <b>Sucesso</b>", Notifica.Sucesso);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("A <b>Lista</b> enviada <b>Não</b> possui registros", Notifica.Alerta);
                    return RedirectToAction(nameof(Index));
                case HttpStatusCode.Conflict:
                    Notificar("A <b>Lista</b> enviada é <b>Inválida</b>", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("A <b>Lista</b> enviada não foi <b>Encontrada</b>", Notifica.Erro);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> ao registrar a Lista de <b>Frequência</b>.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(RegistrarFrequencia), new { idEnsaio = listaFrequencia.First().IdEnsaio });
        }

        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> EnsaiosAssociado ()
        {
            var model = await _ensaio.GetEnsaiosByIdPesoaAsync(Convert.ToInt32(User.FindFirst("Id")?.Value));

            return View(model);
        }

        [Authorize(Roles = "ASSOCIADO")]
        public async Task<ActionResult> JustificarAusencia(int idEnsaio)
        {
            var model = await _ensaio.GetEnsaioPessoaAsync(idEnsaio, Convert.ToInt32(User.FindFirst("Id")?.Value));
            if(model == null)
            {
                return RedirectToAction(nameof(EnsaiosAssociado));
            }
            EnsaioJustificativaViewModel ensaioJustificativa = new()
            {
                IdEnsaio = model.IdEnsaio,
                Justificativa = model.JustificativaFalta
            };
            return View(ensaioJustificativa);
        }

        [Authorize(Roles = "ASSOCIADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> JustificarAusencia(EnsaioJustificativaViewModel ensaioJustificativa)
        {
            if (ModelState.IsValid)
            {
                switch (await _ensaio.RegistrarJustificativaAsync(ensaioJustificativa.IdEnsaio, Convert.ToInt32(User.FindFirst("Id")?.Value), ensaioJustificativa.Justificativa))
                {
                    case HttpStatusCode.OK:
                        Notificar("<b>Justificativa</b> registrada com <b>Sucesso</b>", Notifica.Sucesso);
                        return RedirectToAction(nameof(EnsaiosAssociado));
                    case HttpStatusCode.NotFound:
                        Notificar("A <b>Justificativa</b> enviada é <b>Inválida</b>", Notifica.Erro);
                        break;
                    case HttpStatusCode.Unauthorized:
                        Notificar("Desculpe, <b>Não</b> foi possível <b>Registrar</b> a <b>Justificativa</b>", Notifica.Erro);
                        break;
                    case HttpStatusCode.InternalServerError:
                        Notificar("Desculpe, ocorreu um <b>Erro</b> ao registrar a <b>Justificativa</b>, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        break;
                }
            }
            
            return View(ensaioJustificativa);
        }
    }
}
