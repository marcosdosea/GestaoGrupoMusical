using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using static GestaoGrupoMusicalWeb.Controllers.BaseController;


namespace GestaoGrupoMusicalWeb.Controllers
{
    public class GrupoMusicalController : BaseController
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
        public async Task<ActionResult> Create(GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var grupoModel = _mapper.Map<Grupomusical>(grupoMusicalViewModel);


                switch (await _grupoMusical.Create(grupoModel))
                {
                    case 200:

                        Notificar("Grupo <b> Cadastrado </b> com <b> Sucesso </b> ", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                        break;
                    case 500:
                        Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        return RedirectToAction(nameof(Index));
                        break;
                }

            }
            else
            {
                return View(grupoMusicalViewModel);
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
        public async Task<ActionResult> Edit(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {
            grupoMusicalViewModel.Cnpj = grupoMusicalViewModel.Cnpj.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
            grupoMusicalViewModel.Cep = grupoMusicalViewModel.Cep.Replace("-", string.Empty);
            if (ModelState.IsValid)
            {
                var grupoMusical = _mapper.Map<Grupomusical>(grupoMusicalViewModel);

                switch(await _grupoMusical.Edit(grupoMusical))
                {
                    case 200:
                        Notificar("Grupo <b> Editado </b> com <b> Sucesso </b> ", Notifica.Sucesso);
                        return RedirectToAction(nameof(Index));
                        break;
                    case 500:
                        Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                        return RedirectToAction(nameof(Index));
                        break;
                }
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
        public async  Task<ActionResult> Delete(int id, GrupoMusicalViewModel grupoMusicalViewModel)
        {
            
            switch(await _grupoMusical.Delete(id))
            {
                case 200:
                    Notificar("Grupo <b> Excluido </b> com <b> Sucesso </b> ", Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                    break;
                case 500:
                    Notificar("<b>Erro</b> ! Desculpe, ocorreu um erro durante o <b>Cadastro</b> do associado, se isso persistir entre em contato com o suporte", Notifica.Erro);
                    return RedirectToAction(nameof(Index));
                    break;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
