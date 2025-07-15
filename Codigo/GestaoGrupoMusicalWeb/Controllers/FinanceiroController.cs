using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net;
using static GestaoGrupoMusicalWeb.Controllers.BaseController;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class FinanceiroController : BaseController
    {
        private readonly IFinanceiroService _financeiroService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IPessoaService _pessoaService;


        public FinanceiroController(
            IFinanceiroService financeiroService, 
            IMapper mapper, 
            IGrupoMusicalService grupoMusical,
            IPessoaService pessoaService,
            ILogger<BaseController> logger)
                : base(logger)
        {
            _financeiroService = financeiroService;
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _pessoaService = pessoaService;
        }


        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        [HttpPost]
        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            var listaReceitaFinanceira = _financeiroService.GetAllFinanceiroPorIdGrupo(idGrupoMusical);

            var response = _financeiroService.GetDataPage(request, listaReceitaFinanceira);

            return Json(response);
        }

        // GET: Pagamento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pagamento/Create
        public ActionResult Create()
        {
            return View(new FinanceiroCreateViewModel() { DataInicio = DateTime.Now.Date });
        }

        // POST: Pagamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinanceiroCreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
                model.IdGrupoMusical = idGrupoMusical;
                FinanceiroCreateDTO rf = _mapper.Map<FinanceiroCreateDTO>(model);


                switch (_financeiroService.Create(rf))
                {
                    case FinanceiroStatus.Success:
                        Notificar("<b>Sucesso</b>! Pagamento criado com sucesso!", Notifica.Sucesso);
                        RedirectToAction(nameof(Index));
                        break;
                    case FinanceiroStatus.DataInicioMaiorQueDataFim:
                        Notificar("<b>Erro</b>! A data de <b>inicio</b> deve ser maior que a data <b>fim</b>!!", Notifica.Alerta);
                        RedirectToAction(nameof(Create));
                        break;
                    case FinanceiroStatus.DataFimMenorQueDataDeHoje:
                        Notificar("<b>Erro</b>! A data <b>fim</b> deve ser maior que a data de <b>hoje</b>!!", Notifica.Alerta);
                        RedirectToAction(nameof(Create));
                        break;
                    case FinanceiroStatus.ValorZeroOuNegativo:
                        Notificar("<b>Erro</b>! O <b>valor</b> deve ser <b>positivo</b>!!", Notifica.Alerta);
                        RedirectToAction(nameof(Create));
                        break;
                    default:
                        Notificar("<b>Erro</b>! Algo deu errado na criação do pagamento!", Notifica.Erro);
                        RedirectToAction(nameof(Create));
                        break;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Pagamento/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pagamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Pagamento/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pagamento/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> NotificarFinanceiroViaEmail(int id)
        {
            var pessoas = await _grupoMusicalService.GetAllPeopleFromGrupoMusical(await _grupoMusicalService.GetIdGrupo(User.Identity.Name));
            switch (_financeiroService.NotificarFinanceiroViaEmail(pessoas, id))
            {
                case HttpStatusCode.OK:
                    Notificar("Notificação de Pagamento foi <b>Enviada</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.PreconditionFailed:
                    Notificar("O Pagamento <b>Não</b> está <b>Cadastrado</b> no sistema.", Notifica.Erro);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar($"O pagamento {id} <b>não foi encontrado</b>.", Notifica.Erro);
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

    }
}

