using AutoMapper;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestaoGrupoMusicalWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
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
                return RedirectToAction(nameof(Index), "InstrumentoMusical");
            }

            var listaEvento = _evento.GetAllDTO();
            var EventoViewDTO = _mapper.Map<List<EventoViewModelDTO>>(listaEvento);
            var listaEnsaio = await _ensaioService.GetAllDTO();
            var EnsaioViewDTO = _mapper.Map<List<EnsaioViewModelDTO>>(listaEnsaio);
            var listaIformativo = await _informativoService.GetAllDTO();
            var InformativoDTO = _mapper.Map<List<InformativoViewModelDTO>>(listaIformativo);


            var viewModel = new TelaPrincipalViewModel
            {
                Ensaio = EnsaioViewDTO,
                Evento = EventoViewDTO,
                Informativo = InformativoDTO
            };


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