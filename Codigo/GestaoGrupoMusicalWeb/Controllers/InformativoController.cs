using AutoMapper;
using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Ocsp;
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

        public InformativoController(
            IInformativoService informativoService, 
            IPessoaService pessoaService, 
            IGrupoMusicalService grupoMusicalService, 
            IMapper mapper,
            ILogger<BaseController> logger)
                : base(logger)

        {
            _informativoService = informativoService;
            _pessoaService = pessoaService;
            _grupoMusicalService = grupoMusicalService;
            _mapper = mapper;
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, ASSOCIADO, COLABORADOR")]
        public  ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, ASSOCIADO, COLABORADOR")]
        public async Task<IActionResult> GetDataPage(DatatableRequest request)
        {
            int idGrupoMusical = await _grupoMusicalService.GetIdGrupo(User.Identity.Name);
            var listaInformativo = _informativoService.GetAllInformativoServicePorIdGrupoMusical(idGrupoMusical);
            var listaInformativoDTO = _mapper.Map<List<InformativoIndexDTO>>(listaInformativo);
            var response = _informativoService.GetDataPage(request, listaInformativoDTO);
            return Json(response);
        }

        // GET: InformativoController/Details/5
        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, ASSOCIADO, COLABORADOR")]
        public async Task<ActionResult> Details(uint id)
        {
            var informativo = await _informativoService.Get(id); 
            var model = _mapper.Map<InformativoViewModel>(informativo); 
            return View(model);
        }


        // GET: InformativoController/Create
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
        public ActionResult Create()
        {
            var model = new InformativoViewModel();
            model.Data = DateTime.Now;
            model.EntregarAssociadosAtivos = 1;
            return View(model);
        }

        // POST: InformativoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
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
            }
            else
            {

                Notificar("<b>Erro</b>! Algo deu errado", Notifica.Erro);
                return View();
            }


        }

        // GET: InformativoController/Edit/5
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
        public async Task<ActionResult> Edit(uint id)
        {
            var informativo = await _informativoService.Get(id); 

            if (informativo == null)
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
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
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
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
        public async Task<ActionResult> Delete(uint id)
        {
            if (await _informativoService.Delete(id) == HttpStatusCode.OK)
            {
                Notificar("Informativo <b>Excluído</b> com <b>Sucesso</b>.", Notifica.Sucesso);
            }
            else
            {
                Notificar($"Nenhum <b>Informativo</b> foi encontrado <b>{id}</b>.", Notifica.Erro);
            }
            return RedirectToAction(nameof(Index));
        }


        // POST: InformativoController/GetDataPage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR GRUPO, REGENTE, COLABORADOR")]
        public async Task<IActionResult> NotificarInformativoViaEmail(uint id)
        {
            var pessoas = await _grupoMusicalService.GetAllPeopleFromGrupoMusical(await _grupoMusicalService.GetIdGrupo(User.Identity.Name));
            switch (await _informativoService.NotificarInformativoViaEmail(pessoas, id, "enviar mensagem"))
            {
                case HttpStatusCode.OK:
                    Notificar("Notificação de informativo foi <b>Enviada</b> com <b>Sucesso</b>.", Notifica.Sucesso);
                    break;
                default:
                    Notificar("Não foi permitido enviar Informativo!", Notifica.Sucesso);
                    break;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}