using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
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
        public async Task<ActionResult> Details(uint id)
        {
            var informativo = _informativoService.Get(id);
            var model = _mapper.Map<InformativoViewModel>(informativo);
            return View(model);
        }

        // GET: InformativoController/Create
        public ActionResult Create()
        {
            var model = new InformativoViewModel();
            model.Data = DateTime.Now.Date;
            model.EntregarAssociadosAtivos = 1;
            return View(model);
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
                    var pessoas = await _grupoMusicalService.GetAllPeopleFromGrupoMusical(await _grupoMusicalService.GetIdGrupo(User.Identity.Name));
                    var temp = await _informativoService.NotificarInformativoViaEmail(pessoas, informativo.Id, informativo.Mensagem);
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
        public ActionResult Edit(uint id)
        {
            var informativo = _informativoService.Get(id);

            if(informativo == null)
            {
                Notificar("Erro! O <strong>informativo<strong/> não foi encontrado.", Notifica.Erro);
                return RedirectToAction(nameof(Index));
            }
            var model = _mapper.Map<InformativoViewModel>(informativo);
            Console.WriteLine(model.Data.ToString());
            return View(model);
        }

        // POST: InformativoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InformativoViewModel informativoModel)
        {
            if (ModelState.IsValid)
            {
                Informativo informativo = _mapper.Map<Informativo>(informativoModel);
                if (_informativoService.Edit(_mapper.Map<Informativo>(informativo)) == HttpStatusCode.OK)
                {
                    Notificar("O <b>informativo</b> foi editado com <b>Sucesso</b>!", Notifica.Sucesso);
                }
                else
                {
                    Notificar("Erro! Não foi <strong>permitido<strong/> a edição do<strong>Informativo<strong/>!", Notifica.Sucesso);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: InformativoController/Delete/5
        public async Task<ActionResult> Delete(uint id)
        {
            var informativo =  _informativoService.Get(id);
            var model = _mapper.Map<InformativoViewModel>(informativo);

            return View(model);
        }

        // POST: InformativoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(InformativoViewModel model)
        {
            _informativoService.Delete(model.Id);
            return RedirectToAction(nameof(Index));
        }

    }
}