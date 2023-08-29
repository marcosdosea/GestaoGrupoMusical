using AutoMapper;
using Core;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class ColaboradorController : BaseController
    {
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;

        public ColaboradorController(IPessoaService pessoaService, IMapper mapper)
        {
            _pessoaService = pessoaService;
            _mapper = mapper;
        }

        // GET: ColaboradorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ColaboradorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Muda o papel de uma pessoa para colaborador
        /// </summary>
        /// <param name="id">id do alvo</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: ColaboradorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int Id, int IdPapelGrupo,CreateColaboradorViewModel pessoa)
        {
            HttpStatusCode resul = await _pessoaService.ToCollaborator(Id, IdPapelGrupo);

            switch (resul)
            {
                case HttpStatusCode.Created:
                    Notificar("<b>Sucesso</b>! Associado promovido.", Notifica.Sucesso);
                    break;
                case HttpStatusCode.NotFound:
                    Notificar("<b>Erro</b>! Associado não encontrado.", Notifica.Erro);
                    break;
                case HttpStatusCode.InternalServerError:
                    Notificar("<b>Erro</b>! Algum problema no servidor.", Notifica.Erro);
                    break;
                default:
                    Notificar("<b>Alerta</b>! Ocorreu um erro desconhecido", Notifica.Alerta);
                    break;
            }

            return RedirectToAction("IndexAdmGrupo", "GrupoMusical");
        }

        // GET: ColaboradorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ColaboradorController/Edit/5
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

        /// <summary>
        /// remove o papel de colaborador e 
        /// volta a ser associado
        /// </summary>
        /// <param name="id">id do alvo</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.Get(id);
            var pessoaViewModel = _mapper.Map<PessoaViewModel>(pessoa);

            return View(pessoaViewModel);
        }

        // POST: ColaboradorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CreateColaboradorViewModel pessoa)
        {
            _pessoaService.RemoveCollaborator(id);

            return RedirectToAction("Index", "Pessoa");
        }
    }
}
