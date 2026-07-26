using AutoMapper;
using Core.DTO;
using Core.Service;
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
            if (listaEnsaios == null) return NotFound();

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
                Eventos = new List<EventoAssociadoDTO>()
            };

            return Ok(response);
        }

        // POST: api/Ensaio/JustificarAusencia
        [HttpPost("JustificarAusencia")]
        public async Task<ActionResult> JustificarAusencia([FromBody] JustificarAusenciaEnsaioDTO dto)
        {
            var claimId = User.FindFirst("Id")?.Value;
            int.TryParse(claimId, out int idPessoa);

            if (idPessoa <= 0) return BadRequest(new { mensagem = "Usuário inválido." });

            var resultado = await ensaioService.RegistrarJustificativaAsync(dto.IdEnsaio, idPessoa, dto.Justificativa);

            if (resultado == HttpStatusCode.OK)
            {
                return Ok(new { mensagem = "Justificativa enviada com sucesso!" });
            }

            return BadRequest(new { mensagem = "Não foi possível registrar a justificativa." });
        }

        // GET: api/Ensaio/DetalhesSolicitacao/{idEnsaio}
        [HttpGet("DetalhesSolicitacao/{idEnsaio}")]
        public async Task<ActionResult> GetDetalhesSolicitacao(int idEnsaio)
        {
            var claimId = User.FindFirst("Id")?.Value;
            int.TryParse(claimId, out int idPessoa);

            if (idPessoa <= 0) return BadRequest(new { mensagem = "Usuário inválido." });

            var ensaioPessoa = await ensaioService.GetEnsaioPessoaAsync(idEnsaio, idPessoa);

            if (ensaioPessoa == null) return NotFound(new { mensagem = "Registro não encontrado." });

            return Ok(new
            {
                idEnsaio = ensaioPessoa.IdEnsaio,
                idPessoa = ensaioPessoa.IdPessoa,
                presente = ensaioPessoa.Presente,
                justificativa = ensaioPessoa.JustificativaFalta,
                justificativaAceita = ensaioPessoa.JustificativaAceita
            });
        }
    }
}