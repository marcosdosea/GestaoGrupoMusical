using AutoMapper;
using Core;
using Core.Datatables;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class FinanceiroController : BaseController
    {
        private readonly IFinanceiroService _financeiroService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IPessoaService _pessoaService;


        public FinanceiroController(IFinanceiroService financeiroService, IMapper mapper, IGrupoMusicalService grupoMusical,
            IPessoaService pessoaService)
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
            return View(new FinanceiroViewModel());
        }

        // POST: Pagamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinanceiroViewModel model)
        {

            if (ModelState.IsValid)
            {
                int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
                model.IdGrupoMusical = idGrupoMusical;
                Receitafinanceira rf = _mapper.Map<Receitafinanceira>(model);
                switch (_financeiroService.Create(rf))
                {
                    case FinanceiroStatus.Success:
                        Notificar("<b>Sucesso</b>! Pagamento criado com sucesso!", Notifica.Sucesso);
                        RedirectToAction(nameof(Index));
                        break;
                    case FinanceiroStatus.Error:
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
    }
}
