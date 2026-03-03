using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;


namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnsaioController : ControllerBase
    {

        private readonly IEnsaioService ensaioService;
        private readonly IMapper mapper;

        public EnsaioController(IEnsaioService ensaioService, IMapper mapper)
        {
            this.ensaioService = ensaioService;
            this.mapper = mapper;
        }

        // GET: api/<EnsaioController>
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {   

            var listaEnsaios = await ensaioService.GetAllIndexDTO(1);
            if(listaEnsaios == null) return NotFound();
            
            var listaDto = mapper.Map<IEnumerable<EnsaioIndexDTO>>(listaEnsaios);
            return Ok(listaDto);
           
        }

        // GET api/<EnsaioController>/5
        [HttpGet("{id}")]
        public ActionResult GetAsync(int id)
        {
            var ensaioDetails = ensaioService.GetDetails(id);

            if (ensaioDetails == null) return NotFound();

            var ensaioItem = mapper.Map<EnsaioAssociadoDTO>(ensaioDetails);

            var response = new EventosEnsaiosAssociadoDTO
            {
                Ensaios = new List<EnsaioAssociadoDTO> { ensaioItem },
                Eventos = new List<EventoAssociadoDTO>() // Lista vazia ou vinda de outro serviço
            };

            return Ok(response);
        }
       
    }
}
