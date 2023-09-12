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
             
            return RedirectToAction("Index", "Ensaio");
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