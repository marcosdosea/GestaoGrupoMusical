using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


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

        [Authorize]
        [HttpGet("DetalhesSolicitacao/{idEnsaio}")]
        public async Task<ActionResult> GetDetalhesSolicitacao(int idEnsaio)
        {
            var idPessoa = ObterIdPessoaLogada();
            if (idPessoa <= 0)
                return Unauthorized(new { mensagem = "Não foi possível identificar o associado autenticado." });

            var frequencia = await ensaioService.GetEnsaioPessoaAsync(idEnsaio, idPessoa);
            if (frequencia == null)
                return NotFound(new { mensagem = "Você não está vinculado a este ensaio." });

            return Ok(new
            {
                idEnsaio,
                justificativa = frequencia.JustificativaFalta,
                presente = frequencia.Presente == 1,
                justificativaAceita = frequencia.JustificativaAceita == 1
            });
        }

        [Authorize]
        [HttpPost("JustificarAusencia")]
        public async Task<ActionResult> JustificarAusencia([FromBody] JustificativaAusenciaDTO dto)
        {
            if (!ModelState.IsValid || dto.IdEnsaio <= 0)
                return BadRequest(new { mensagem = "Informe um ensaio e uma justificativa válida." });

            var idPessoa = ObterIdPessoaLogada();
            if (idPessoa <= 0)
                return Unauthorized(new { mensagem = "Não foi possível identificar o associado autenticado." });

            var resultado = await ensaioService.RegistrarJustificativaAsync(
                dto.IdEnsaio,
                idPessoa,
                dto.Justificativa!.Trim());

            return resultado switch
            {
                HttpStatusCode.OK => Ok(new { mensagem = "Justificativa registrada com sucesso." }),
                HttpStatusCode.NotFound => NotFound(new { mensagem = "Você não está vinculado a este ensaio." }),
                _ => StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensagem = "Não foi possível registrar a justificativa." })
            };
        }

        private int ObterIdPessoaLogada()
        {
            return int.TryParse(User.FindFirst("IdPessoa")?.Value, out var idPessoa) ? idPessoa : 0;
        }
       
    }
}
