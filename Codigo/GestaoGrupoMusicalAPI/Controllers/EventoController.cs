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
        [AllowAnonymous]
        [HttpGet("Detalhes/{id}")]
        public async Task<ActionResult> GetDetalhesSolicitacao(int id)
        {
            // 1. Pega o valor do Token
            var claimId = User.FindFirst("Id")?.Value;

            // 2. Tenta converter. Se for GUID, idPessoa será 0
            int.TryParse(claimId, out int idPessoa);

            // 3. Busca o evento
            var evento = eventoService.Get(id);
            if (evento == null) return NotFound();

            var instrumentos = await eventoService.GetInstrumentosDisponiveisAsync(id);

            // 4. Busca a inscrição apenas se o ID for válido (> 0)
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
                inscricao = minhaInscricao
            });
        }

        // POST: api/Evento/ResponderPresenca
        [HttpPost("ResponderPresenca")]
        public async Task<ActionResult> ResponderPresenca([FromBody] SolicitarParticipacaoDTO dto)
        {
            // 1. Pega o valor do Token (que está vindo como GUID)
            var claimId = User.FindFirst("Id")?.Value;

            // 2. Tenta converter. Se falhar, tenta usar a claim "IdPessoa" (caso você tenha configurado)
            if (!int.TryParse(claimId, out int idPessoa))
            {
                // Fallback: se o "Id" for GUID, talvez você tenha o ID numérico em outra claim
                var claimIdPessoa = User.FindFirst("IdPessoa")?.Value;
                int.TryParse(claimIdPessoa, out idPessoa);
            }

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
        [HttpPost("CancelarPresenca/{idEvento}")]
        public async Task<ActionResult> CancelarPresenca(int idEvento)
        {
            var claimId = User.FindFirst("Id")?.Value;
            int.TryParse(claimId, out int idPessoa);

            if (idPessoa <= 0) return BadRequest(new { mensagem = "Usuário inválido." });

            var resultado = await eventoService.CancelarSolicitacao(idEvento, idPessoa);

            if (resultado == HttpStatusCode.OK)
                return Ok(new { mensagem = "Solicitação cancelada." });

            return BadRequest(new { mensagem = "Não foi possível cancelar. Verifique se a solicitação já foi aprovada." });
        }
    }
}