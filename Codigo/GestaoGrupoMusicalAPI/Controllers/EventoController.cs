using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net;

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IEventoService eventoService;
        private readonly IMapper mapper;

        public EventoController(IEventoService eventoService, IMapper mapper)
        {
            this.eventoService = eventoService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var listaEvento = await eventoService.GetAllDTOAsync();
            if (listaEvento == null) return NotFound("Nenhum evento encontrado");
            return Ok(listaEvento);
        }

        // GET: api/Evento/Detalhes/5
        // Retorna tudo o que a tela de "Aceitar" do Mobile precisa
        [Authorize]
        [HttpGet("Detalhes/{id}")]
        public async Task<ActionResult> GetDetalhesSolicitacao(int id)
        {
            var idPessoa = ObterIdPessoaLogada();

            // 3. Busca o evento
            var evento = eventoService.Get(id);
            if (evento == null) return NotFound();

            var instrumentos = await eventoService.GetInstrumentosDisponiveisAsync(id);

            var minhaInscricao = idPessoa > 0
                ? await eventoService.GetSolicitacaoAssociado(id, idPessoa)
                : null;

            // 5. Retorna 200 OK com mapeamento idêntico ao EventoModel do Flutter
            return Ok(new
            {
                id = evento.Id,
                dataHoraInicio = evento.DataHoraInicio,
                dataHoraFim = evento.DataHoraFim,
                repertorio = evento.Repertorio,
                local = evento.Local,
                instrumentosDisponiveis = instrumentos,
                inscricao = minhaInscricao,
                minhaInscricao
            });
        }

        [Authorize]
        [HttpPost("JustificarAusencia")]
        public async Task<ActionResult> JustificarAusencia([FromBody] JustificativaAusenciaDTO dto)
        {
            if (!ModelState.IsValid || dto.IdEvento <= 0)
                return BadRequest(new { mensagem = "Informe um evento e uma justificativa válida." });

            var idPessoa = ObterIdPessoaLogada();
            if (idPessoa <= 0)
                return Unauthorized(new { mensagem = "Não foi possível identificar o associado autenticado." });

            var resultado = await eventoService.RegistrarJustificativaAsync(
                dto.IdEvento,
                idPessoa,
                dto.Justificativa!.Trim());

            return resultado switch
            {
                HttpStatusCode.OK => Ok(new { mensagem = "Justificativa registrada com sucesso." }),
                HttpStatusCode.NotFound => NotFound(new { mensagem = "Você não está vinculado a este evento." }),
                HttpStatusCode.BadRequest => BadRequest(new { mensagem = "A justificativa só pode ser enviada após a aprovação da participação." }),
                _ => StatusCode(StatusCodes.Status500InternalServerError,
                    new { mensagem = "Não foi possível registrar a justificativa." })
            };
        }

        // POST: api/Evento/ResponderPresenca
        [Authorize]
        [HttpPost("ResponderPresenca")]
        public async Task<ActionResult> ResponderPresenca([FromBody] SolicitarParticipacaoDTO dto)
        {
            var idPessoa = ObterIdPessoaLogada();

            // 3. Se ainda assim for 0, o sistema não pode prosseguir
            if (idPessoa <= 0)
            {
                return BadRequest(new { mensagem = "Não foi possível identificar o seu perfil de associado. Por favor, saia e entre novamente." });
            }

            if (!await eventoService.PodeAssociadoSolicitar(dto.IdEvento, idPessoa))
            {
                return BadRequest(new { mensagem = "Não é possível solicitar participação neste evento." });
            }

            var resultado = await eventoService.SolicitarParticipacao(
                dto.IdEvento,
                idPessoa,
                dto.IdTipoInstrumento
            );

            if (resultado == HttpStatusCode.OK)
                return Ok(new { mensagem = "Solicitação enviada com sucesso!" });

            if (resultado == HttpStatusCode.Conflict)
                return Conflict(new { mensagem = "Você já possui uma solicitação para este evento." });

            return StatusCode(500, new { mensagem = "Erro interno ao processar solicitação." });
        }

        // POST: api/Evento/CancelarPresenca
        [Authorize]
        [HttpPost("CancelarPresenca/{idEvento}")]
        public async Task<ActionResult> CancelarPresenca(int idEvento)
        {
            var idPessoa = ObterIdPessoaLogada();

            if (idPessoa <= 0) return BadRequest(new { mensagem = "Usuário inválido." });

            var resultado = await eventoService.CancelarSolicitacao(idEvento, idPessoa);

            if (resultado == HttpStatusCode.OK)
                return Ok(new { mensagem = "Solicitação cancelada." });

            return BadRequest(new { mensagem = "Não foi possível cancelar. Verifique se a solicitação já foi aprovada." });
        }

        private int ObterIdPessoaLogada()
        {
            return int.TryParse(User.FindFirst("IdPessoa")?.Value, out var idPessoa) ? idPessoa : 0;
        }
    }
}
