using AutoMapper;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Http;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net.WebSockets;
using AspNetCore;
using System.Web;
namespace GestaoGrupoMusicalWeb.Controllers
{
    public class GrupoMusicalController : Controller
    {
        private readonly IGrupoMusicalService _grupoMusical;
        private readonly IMapper _mapper;

        public GrupoMusicalController(IGrupoMusicalService grupoMusical, IMapper mapper)
        {
            _grupoMusical = grupoMusical;
            _mapper = mapper;
        }



        // GET: GrupoMusicalController
        public ActionResult Index()
        {
            var listaGrupoMusical = _grupoMusical.GetAllDTO();
            return View(listaGrupoMusical);
        }

        // GET: GrupoMusicalController/Details/5
        public ActionResult Details(int id)
        {
            var grupoMusical = _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);
            return View(grupoModel);
        }

        // GET: GrupoMusicalController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GrupoMusicalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var grupoModel = _mapper.Map<Grupomusical>(grupoMusicalViewModel);


                switch (_grupoMusical.Create(grupoModel))
                {
                    case 200:

                        

                        break;
                    case 500:

                        break;

                }

            }
            else
            {

            }
            return RedirectToAction(nameof(Index));

        }

        // GET: GrupoMusicalController/Edit/5
        public ActionResult Edit(int id)
        {
            var grupoMusical = _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);

            return View(grupoModel);
        }

        // POST: GrupoMusicalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var grupoMusical = _mapper.Map<Grupomusical>(grupoMusicalViewModel);
                _grupoMusical.Edit(grupoMusical);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: GrupoMusicalController/Delete/5
        public ActionResult Delete(int id)
        {
            var grupoMusical = _grupoMusical.Get(id);
            var grupoModel = _mapper.Map<GrupoMusicalViewModel>(grupoMusical);
            return View(grupoModel);
        }

        // POST: GrupoMusicalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {
            _grupoMusical.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
