using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

            var listaEnsaios = await ensaioService.GetAll();
            if(listaEnsaios == null)
            {
                return NotFound();
            }

            var listaDto = mapper.Map<IEnumerable<EnsaioIndexDTO>>(listaEnsaios);
            return Ok(listaDto);
           
        }

        // GET api/<EnsaioController>/5
        [HttpGet("{id}")]
        public ActionResult GetAsync(int id)
        {
            Ensaio ensaio = ensaioService.Get(id);

            if(ensaio == null)
            {
                return NotFound();
            }

            var listaDto = mapper.Map<EventosEnsaiosAssociadoDTO>(ensaio);
            return Ok(listaDto);
        }
       
    }
}
