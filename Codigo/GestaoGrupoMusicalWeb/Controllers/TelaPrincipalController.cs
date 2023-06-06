using AspNetCore;
using AutoMapper;
using Core.Service;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public class TelaPrincipalController : Controller
    {
        private readonly IEvento _evento;
        private readonly IEnsaioService _ensaioService;
        private readonly IMapper _mapper;

        public TelaPrincipalController(IEvento evento, IEnsaioService ensaioService, IMapper mapper)
        {
            _evento = evento;
            _ensaioService = ensaioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var listaEvento = _evento.GetAllDTO();
            var EventoViewDTO = _mapper.Map<List<EventoViewModelDTO>>(listaEvento);
            var listaEnsaio = _ensaioService.GetAllDTO();
            var EnsaioViewDTO = _mapper.Map<List<EnsaioViewModelDTO>>(listaEnsaio);

            var viewModel = new TelaPrincipalViewModel
            {
                Ensaio = EnsaioViewDTO,
                Evento = EventoViewDTO
            };


            return View(viewModel);
        }
    }
}
