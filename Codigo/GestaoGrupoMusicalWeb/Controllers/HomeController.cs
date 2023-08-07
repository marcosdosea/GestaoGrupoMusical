using AutoMapper;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static GestaoGrupoMusicalWeb.Controllers.BaseController;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventoService _evento;
        private readonly IEnsaioService _ensaioService;
        private readonly IInformativoService _informativoService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, 
                              IEventoService evento, 
                              IEnsaioService ensaioService, 
                              IInformativoService informativoService,
                              IMapper mapper)
        {
            _logger = logger;
            _evento = evento;
            _ensaioService = ensaioService;
            _informativoService = informativoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {

            var listaEvento = _evento.GetAllDTO();
            var eventoViewDTO = _mapper.Map<List<EventoViewModelDTO>>(listaEvento);
            var listaEnsaio = await _ensaioService.GetAllDTO();
            var ensaioViewDTO = _mapper.Map<List<EnsaioViewModelDTO>>(listaEnsaio);
            var listaIformativo = await _informativoService.GetAll();
            var informativo = _mapper.Map<List<InformativoViewModel>>(listaIformativo);


            var viewModel = new TelaPrincipalViewModel
            {
                Ensaio = ensaioViewDTO,
                Evento = eventoViewDTO,
                Informativo = informativo
            };


            if (User.IsInRole("ADMINISTRADOR SISTEMA"))
            {
                return RedirectToAction(nameof(Index), "GrupoMusical");
            }
            if (!Convert.ToBoolean(User.FindFirst("Ativo")?.Value))
            {
                Notificar("<span class=\"fw-bold fs-5 mt-3\">Erro ! Houve um erro no login, Associado não está Ativo",
       Notifica.Erro);
                return RedirectToAction("Autenticar", "Identity");
            }

            if (User.IsInRole("ASSOCIADO"))
            {
                return RedirectToAction("Movimentacoes", "InstrumentoMusical");
            }
            else if(User.IsInRole("ADMINISTRADOR SISTEMA"))
            {
                return RedirectToAction(nameof(Index), "GrupoMusical");
            }
            else if(User.IsInRole("ADMINISTRADOR GRUPO"))
            {
                //return RedirectToAction(nameof(Index));
                return View(viewModel);

            }

            //var listaEvento = _evento.GetAllDTO();
            //var EventoViewDTO = _mapper.Map<List<EventoViewModelDTO>>(listaEvento);
            //var listaEnsaio = await _ensaioService.GetAllDTO();
            //var EnsaioViewDTO = _mapper.Map<List<EnsaioViewModelDTO>>(listaEnsaio);
            //var listaIformativo = await _informativoService.GetAllDTO();
            //var InformativoDTO = _mapper.Map<List<InformativoViewModelDTO>>(listaIformativo);


            //var viewModel = new TelaPrincipalViewModel
            //{
            //    Ensaio = EnsaioViewDTO,
            //    Evento = EventoViewDTO,
            //    Informativo = InformativoDTO
            //};


            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}