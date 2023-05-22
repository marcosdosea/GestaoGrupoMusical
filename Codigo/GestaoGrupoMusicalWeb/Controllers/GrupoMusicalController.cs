using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class GrupoMusicalController : Controller
    {
        // GET: GrupoMusicalController
        public ActionResult Index()
        {
            return View();
        }

        // GET: GrupoMusicalController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GrupoMusicalController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GrupoMusicalController/Create
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

        // GET: GrupoMusicalController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GrupoMusicalController/Edit/5
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

        // GET: GrupoMusicalController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GrupoMusicalController/Delete/5
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
