using AutoMapper;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{

    public class ApresentacaoController : Controller
    {
        private readonly IApresentacao _apresentacao;
        private readonly IMapper _mapper;


        // GET: ApresentacaoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ApresentacaoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ApresentacaoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApresentacaoController/Create
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

        // GET: ApresentacaoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ApresentacaoController/Edit/5
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

        // GET: ApresentacaoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApresentacaoController/Delete/5
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
