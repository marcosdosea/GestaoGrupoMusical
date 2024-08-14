using AutoMapper;
using Core;
using Core.Datatables;
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
        private readonly IFigurinoService _figurino;
        private readonly IGrupoMusicalService _grupoMusical;

        public EnsaioController(IMapper mapper, IEnsaioService ensaio, IPessoaService pessoa, IFigurinoService figurino, IGrupoMusicalService grupoMusical)
        {
            _ensaio = ensaio;
            _mapper = mapper;
            _pessoa = pessoa;
            _figurino = figurino;
            _grupoMusical = grupoMusical;
        }
        [HttpPost]
        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            var ensaios = await _ensaio.GetDataPage(request, await _grupoMusical.GetIdGrupo(User.Identity.Name));
            return Json(ensaios);
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

            var figurino = await _figurino.GetAllFigurinoDropdown(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));

            if (figurino == null || !figurino.Any())
            {
                Notificar("É necessário cadastrar um Figurino para então cadastrar um Ensaio.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            EnsaioViewModel ensaioModel = new()
            {
                ListaPessoa = new SelectList(lista, "Id", "Nome"),
                ListaFigurino = new SelectList(figurino, "Id", "Nome")
            };

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
                switch (await _ensaio.Create(ensaio, ensaioViewModel.IdRegentes, ensaioViewModel.IdFigurinoSelecionado))
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
            var ensaio = _ensaio.Get(id);
            if (Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value) != ensaio.IdGrupoMusical)
            {
                Notificar("<b>Ensaio não encontrado!</b>", Notifica.Alerta);
                return RedirectToAction(nameof(Index));
            }
            var listaRegentes = await _pessoa.GetRegentesForAutoCompleteAsync(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));
            var listaFigurinos = await _figurino.GetAllFigurinoDropdown(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));

            EnsaioViewModel ensaioModel = _mapper.Map<EnsaioViewModel>(ensaio);

            ensaioModel.ListaPessoa = new SelectList(listaRegentes, "Id", "Nome");
            ensaioModel.ListaFigurino = new SelectList(listaFigurinos, "Id", "Nome");

            ViewData["exemploRegente"] = listaRegentes.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            ViewData["jsonIdRegentes"] = (await _ensaio.GetIdRegentesEnsaioAsync(id)).ToJson();
            ensaioModel.JsonLista = listaRegentes.ToJson();
            return View(ensaioModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // POST: EnsaioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnsaioViewModel ensaioViewModel)
        {
            if (ModelState.IsValid && ensaioViewModel.IdRegentes != null)
            {
                String mensagem = String.Empty;
                switch (await _ensaio.Edit(_mapper.Map<Ensaio>(ensaioViewModel), ensaioViewModel.IdRegentes))
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
                    case HttpStatusCode.NotFound:
                        mensagem = "<b>Erro</b> ! <b>Não</b> foi possível <b>Editar</b> esse <b>Ensaio</b>.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                    case HttpStatusCode.InternalServerError:
                        mensagem = "<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Editar</b> de ensaio.";
                        Notificar(mensagem, Notifica.Erro);
                        break;
                }
            }
            var lista = await _pessoa.GetRegentesForAutoCompleteAsync(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));
            var listaFigurinos = await _figurino.GetAllFigurinoDropdown(Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value));

            ensaioViewModel.ListaPessoa = new SelectList(lista, "Id", "Nome");
            ensaioViewModel.ListaFigurino = new SelectList(listaFigurinos, "Id", "Nome");

            ViewData["exemploRegente"] = lista.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            ViewData["jsonIdRegentes"] = (await _ensaio.GetIdRegentesEnsaioAsync(ensaioViewModel.Id)).ToJson();
            ensaioViewModel.JsonLista = lista.ToJson();
            return View(ensaioViewModel);
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var ensaio = _ensaio.Get(id);
            return View(_mapper.Map<EnsaioViewModel>(ensaio));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // POST: EnsaioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(EnsaioViewModel ensaioModel)
        {
            String mensagem = String.Empty;
            switch (_ensaio.Delete(ensaioModel.Id))
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

        public async Task<ActionResult> NotificarEnsaioViaEmail(int id)
        {
            var pessoas = await _grupoMusical.GetAllPeopleFromGrupoMusical(await _grupoMusical.GetIdGrupo(User.Identity.Name));
            switch (await _ensaio.NotificarEnsaioViaEmail(pessoas, id))
            {
                case HttpStatusCode.OK:
                    Notificar("Notificação de Ensaio foi <b>Enviada</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("O Ensaio <b>Não</b> está <b>Cadastrado</b> no sistema.", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar($"O Ensaio {id} <b>não foi encontrado</b>.", Notifica.Erro);
                    break;
                case HttpStatusCode.BadRequest:
                    Notificar("Houve um erro. Tente novamente mais tarde.", Notifica.Alerta);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("Desculpe, ocorreu um <b>Erro</b> durante o <b>Envio</b> da notificação.", Notifica.Erro);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO,COLABORADOR,REGENTE")]
        // GET: EnsaioController/RegistrarFrequencia
        public async Task<ActionResult> RegistrarFrequencia(int idEnsaio)
        {
            //idEnsaio = 1;
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("Ensaio: " + idEnsaio);
            Console.WriteLine("-----------------------------------------------------------------------");

            int idGrupoMusical = await _grupoMusical.GetIdGrupo(User.Identity.Name);

            var listaRegentes = await _pessoa.GetRegentesForAutoCompleteAsync(idGrupoMusical);
            if (listaRegentes == null || !listaRegentes.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Regente para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var listaFigurinos = await _figurino.GetAllFigurinoDropdown(idGrupoMusical);

            if (listaFigurinos == null || !listaFigurinos.Any())
            {
                Notificar("É necessário cadastrar pelo menos um Figurino para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }

            var listaAssociadosAtivos = await _pessoa.GetAssociadoAtivos(idGrupoMusical);

            if(listaAssociadosAtivos == null || !listaAssociadosAtivos.Any())
            {
                Notificar("É necessário pelo menos um Associado Ativo para então registrar uma frequência.", Notifica.Informativo);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ListaAssociadosAtivos = listaAssociadosAtivos;

            var ensaio = _ensaio.Get(idEnsaio);

            EnsaioViewModel ensaioView = _mapper.Map<EnsaioViewModel>(ensaio);

            ensaioView.ListaPessoa = new SelectList(listaRegentes, "Id", "Nome");
            ensaioView.ListaFigurino = new SelectList(listaFigurinos, "Id", "Nome");
            ensaioView.ListaAssociadosAtivos = new SelectList(listaAssociadosAtivos, "Id", "Nome", "Cpf");

            ViewData["exemploRegente"] = listaRegentes.Select(p => p.Nome).FirstOrDefault()?.Split(" ")[0];
            ViewData["jsonIdRegentes"] = (await _ensaio.GetIdRegentesEnsaioAsync(ensaioView.Id)).ToJson();
            ensaioView.JsonLista = listaRegentes.ToJson();

            return View(ensaioView);
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