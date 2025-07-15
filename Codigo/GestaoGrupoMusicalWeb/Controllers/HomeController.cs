using AutoMapper;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Claims;

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

        public HomeController(
            ILogger<HomeController> logger,
            IEventoService evento,
            IEnsaioService ensaioService,
            IInformativoService informativoService,
            IMapper mapper,
            ILogger<BaseController> baselogger)
                : base(baselogger)
        {
            _logger = logger;
            _evento = evento;
            _ensaioService = ensaioService;
            _informativoService = informativoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (!Convert.ToBoolean(User.FindFirst("Ativo")?.Value))
            {
                Notificar("Erro! Houve um erro no login, Associado não está Ativo.", Notifica.Erro);
                return RedirectToAction("Autenticar", "Identity");
            }

            if (User.IsInRole("ASSOCIADO"))
            {
                return RedirectToAction("Movimentacoes", "InstrumentoMusical");
            }

            
            var todosEventos = _evento.GetAll();                      
            var todosEnsaios = await _ensaioService.GetAll();         
            var todosInformativos = await _informativoService.GetAll(); 

            
            var eventosDoGrupo = todosEventos.Where(e => e.IdGrupoMusical == IdGrupoMusical);
            var ensaiosDoGrupo = todosEnsaios.Where(e => e.IdGrupoMusical == IdGrupoMusical);
            var informativosDoGrupo = todosInformativos.Where(i => i.IdGrupoMusical == IdGrupoMusical);

            
            var eventosViewModel = _mapper.Map<IEnumerable<EventoViewModel>>(eventosDoGrupo);
            var ensaiosViewModel = _mapper.Map<IEnumerable<EnsaioViewModel>>(ensaiosDoGrupo);
            var informativosViewModel = _mapper.Map<IEnumerable<InformativoViewModel>>(informativosDoGrupo);

            var viewModel = new HomeViewModel
            {
                Evento = eventosViewModel.Where(e => e.DataHoraInicio >= DateTime.Now).OrderBy(e => e.DataHoraInicio).Take(5),
                Ensaio = ensaiosViewModel.Where(e => e.DataHoraInicio >= DateTime.Now).OrderBy(e => e.DataHoraInicio).Take(5),
                Informativo = informativosViewModel.OrderByDescending(i => i.Data).Take(5)
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