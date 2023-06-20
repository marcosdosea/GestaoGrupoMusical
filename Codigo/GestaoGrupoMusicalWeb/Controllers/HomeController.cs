using AutoMapper;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEvento _evento;
        private readonly IEnsaioService _ensaioService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IEvento evento, IEnsaioService ensaioService, IMapper mapper)
        {
            _logger = logger;
            _evento = evento;
            _ensaioService = ensaioService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var listaEvento = _evento.GetAllDTO();
            var EventoViewDTO = _mapper.Map<List<EventoViewModelDTO>>(listaEvento);
            var listaEnsaio = await _ensaioService.GetAllDTO();
            var EnsaioViewDTO = _mapper.Map<List<EnsaioViewModelDTO>>(listaEnsaio);


            var viewModel = new TelaPrincipalViewModel
            {
                Ensaio = EnsaioViewDTO,
                Evento = EventoViewDTO
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