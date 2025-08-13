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
        //public ActionResult Details(int id)
        //{
           // return View();
        //}

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

        // FinanceiroController.cs

        // GET: Financeiro/Edit/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult Edit(int id)
        {
            var financeiro = _financeiroService.Get(id);
            var financeiroModel = _mapper.Map<FinanceiroCreateViewModel>(financeiro);
            return View(financeiroModel);
        }

        // POST: Pagamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public async Task<ActionResult> Edit(int id, FinanceiroCreateViewModel model)
        {
            if (id != model.Id)
            {
                Notificar("<b>Erro</b>! ID do pagamento não corresponde.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
                model.IdGrupoMusical = idGrupoMusical;

                
                var financeiro = _mapper.Map<Receitafinanceira>(model);

                switch (_financeiroService.Edit(financeiro))
                {
                    case FinanceiroStatus.Success:
                        Notificar("<b>Sucesso</b>! Pagamento atualizado com sucesso!", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                    case FinanceiroStatus.DataInicioMaiorQueDataFim:
                        Notificar("<b>Erro</b>! A data de <b>início</b> não pode ser maior que a data <b>fim</b>!", Notifica.Alerta);
                        break;
                    case FinanceiroStatus.DataFimMenorQueDataDeHoje:
                        Notificar("<b>Erro</b>! A data <b>fim</b> não pode ser menor que a data de <b>hoje</b>!", Notifica.Alerta);
                        break;
                    case FinanceiroStatus.ValorZeroOuNegativo:
                        Notificar("<b>Erro</b>! O <b>valor</b> deve ser <b>positivo</b>!", Notifica.Alerta);
                        break;
                    default:
                        Notificar("<b>Erro</b>! Algo deu errado na atualização do pagamento!", Notifica.Erro);
                        break;
                }
            }

            // Se o modelo não for válido ou a edição falhar, retorna para a mesma view com os dados preenchidos
            return View(model);
        }
        // GET: Financeiro/Details/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult Details(int id)
        {
            var financeiro = _financeiroService.Get(id);
            if (financeiro == null)
            {
                return NotFound();
            }
            var financeiroModel = _mapper.Map<FinanceiroCreateViewModel>(financeiro);
            return View(financeiroModel);
        }
        // GET: Financeiro/Delete/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult Delete(int id)
        {
            var financeiro = _financeiroService.Get(id);
            var financeiroModel = _mapper.Map<FinanceiroCreateViewModel>(financeiro);
            return View(financeiroModel);
        }

        // POST: Financeiro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, COLABORADOR")]
        public ActionResult DeleteConfirmed(int id)
        {
            _financeiroService.Delete(id);
            Notificar("Pagamento <b>removido</b> com sucesso!", Notifica.Sucesso);
            return RedirectToAction(nameof(Index));
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
        [Authorize(Roles = "ADMINISTRADOR GRUPO, ADM_SISTEMA, COLABORADOR")]
        public async Task<IActionResult> Pag_Associados(int id)
        {
            var receita = _financeiroService.Get(id);
            if (receita == null)
            {
                return NotFound();
            }

            var model = new PagamentoAssociadoViewModel
            {
                Financeiro = _mapper.Map<FinanceiroCreateViewModel>(receita),
                Associados = (await _financeiroService.GetAssociadosPagamento(id)).ToList(),
                IdReceita = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, ADM_SISTEMA, COLABORADOR")]
        public async Task<IActionResult> Pag_Associados(PagamentoAssociadoViewModel model)
        {
            if (ModelState.IsValid && model.Associados != null)
            {
                try
                {
                    await _financeiroService.SalvarPagamentos(model.IdReceita, model.Associados);
                    Notificar("Pagamentos dos associados foram salvos com sucesso!", Notifica.Sucesso);
                }
                catch (Exception)
                {
                    Notificar("Ocorreu um erro ao tentar salvar os pagamentos. Tente novamente.", Notifica.Erro);
                    var receitaErro = _financeiroService.Get(model.IdReceita);
                    model.Financeiro = _mapper.Map<FinanceiroCreateViewModel>(receitaErro);
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }

            var receitaPost = _financeiroService.Get(model.IdReceita);
            model.Financeiro = _mapper.Map<FinanceiroCreateViewModel>(receitaPost);
            Notificar("Ocorreu um erro. Verifique os dados.", Notifica.Erro);
            return View(model);
        }
    }
}

