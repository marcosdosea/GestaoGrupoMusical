using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service;
using System.Net;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class InformativoController : BaseController
    {
        // GET: InformativoController
        private readonly IInformativoService _informativoService;
        private readonly IPessoaService _pessoaService;
        private readonly IGrupoMusicalService _grupoMusicalService;
        private readonly IMapper _mapper;

        public InformativoController(IInformativoService informativoService, IPessoaService pessoaService, IGrupoMusicalService grupoMusicalService, IMapper mapper)
        {
            _informativoService = informativoService;
            _pessoaService = pessoaService;
            _grupoMusicalService = grupoMusicalService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var informativos = await _informativoService.GetAll();
            return View(_mapper.Map<IEnumerable<InformativoViewModel>>(informativos));
        }

        // GET: InformativoController/Details/5
        public async Task<ActionResult> Details(int idPessoa, int idGrupoMusical)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            return View(model);
        }

        // GET: InformativoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InformativoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InformativoViewModel informativoViewModel)
        {
            if (ModelState.IsValid)
            {
                UserDTO user = await _pessoaService.GetByCpf(User.Identity.Name);

                var informativo = new Informativo
                {
                    EntregarAssociadosAtivos = informativoViewModel.EntregarAssociadosAtivos,
                    Data = informativoViewModel.Data,
                    Mensagem = informativoViewModel.Mensagem,
                    IdGrupoMusical = user!.IdGrupoMusical,
                    IdPessoa = user.Id,
                }; 

                var statusCode = await _informativoService.Create(informativo);

                if (statusCode == HttpStatusCode.Created)
                {
                    Notificar("Informativo <b>Cadastrado</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    return RedirectToAction(nameof(Index));
                }
                Notificar("<b>Erro</b>! Há algo errado ao cadastrar Informativo", Notifica.Erro);
                return RedirectToAction("Index");
            } else{

                Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                return View();
            }


        }

        // GET: InformativoController/Edit/5
        public async Task<ActionResult> Edit(int idGrupoMusical, int idPessoa)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);

            return View(model);
        }

        // POST: InformativoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, InformativoViewModel informativo)
        {
            if (ModelState.IsValid)
            {
                if (await _informativoService.Edit(_mapper.Map<Informativo>(informativo)))
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(informativo);

        }

        // GET: InformativoController/Delete/5
        public async Task<ActionResult> Delete(int idGrupoMusical, int idPessoa)
        {
            var informativo = await _informativoService.Get(idGrupoMusical, idPessoa);
            var model = _mapper.Map<InformativoViewModel>(informativo);

            return View(model);
        }

        // POST: InformativoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int idGrupoMusical, int idPessoa, InformativoViewModel informativo)
        {
            await _informativoService.Delete(idGrupoMusical, idPessoa);
            return RedirectToAction(nameof(Index));
        }
    }
}