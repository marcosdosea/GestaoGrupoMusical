﻿using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class FinanceiroController : BaseController
    {
        private readonly IFinanceiroService _financeiroService;
        private readonly IMapper _mapper;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IPessoaService _pessoaService;
        private int ReceitaFinanceiraMesesAtrasados { get; }


        public FinanceiroController(IFinanceiroService financeiroService,IMapper mapper, IGrupoMusicalService grupoMusical,
            IPessoaService pessoaService,IConfiguration configuration)
        {
            _financeiroService = financeiroService;
            _mapper = mapper;
            _grupoMusicalService = grupoMusical;
            _pessoaService = pessoaService;
            ReceitaFinanceiraMesesAtrasados = configuration.GetValue<int>("Aplication:ReceitaFinanceiraMesesAtrasados");
        }


        // GET: Pagamento
        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);

            var listaReceitaFinanceira = await _financeiroService.GetAllFinanceiroPorIdGrupo(idGrupoMusical, ReceitaFinanceiraMesesAtrasados);

            var response = _financeiroService.GetDataPage(request, listaReceitaFinanceira);
            Console.WriteLine("### RECEITA DATAPAGE ### " + listaReceitaFinanceira.Count());
            Console.WriteLine("### RECEITA DATAPAGE ### " + response.Data?.Count());
            
            if(response.Data?.Count() > 0)
            {
                Console.WriteLine("NOT NULL");
                foreach (var item in response.Data)
                {
                    Console.WriteLine("### ITEM ###");
                    Console.WriteLine("ID: " + item.Id);
                    Console.WriteLine("PAGOS: " + item.Pagos);
                    Console.WriteLine("ISENTOS: " + item.Isentos);
                    Console.WriteLine("Atrasos: " + item.Atrasos);
                    Console.WriteLine("Recebido: " + item.Recebido);
                }
            }
            else
            {
                Console.WriteLine("NULL");
            }
            response.Data = null;
            
            return Json(response);
        }

        // GET: Pagamento/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pagamento/Create
        public async Task<ActionResult> Create()
        {
            Console.WriteLine("### CREATE DATAPAGE ###");
            _ = await GetDataPage(new DatatableRequest());
            return RedirectToAction("Index");
            //return View();
        }

        // POST: Pagamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
