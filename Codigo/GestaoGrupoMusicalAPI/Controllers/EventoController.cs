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
    ///[Authorize(Roles = "Associados")] 
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
        public ActionResult Get()
        {
            var listaEvento = eventoService.GetAllDTO();
            if (listaEvento == null) return NotFound();
            return Ok(listaEvento);
        }

        // GET: api/Evento/Detalhes/5
        // Retorna tudo o que a tela de "Aceitar" do Mobile precisa
        [HttpGet("Detalhes/{id}")]
        public async Task<ActionResult> GetDetalhesSolicitacao(int id)
        {
            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);

            var evento = eventoService.Get(id);
            if (evento == null) return NotFound("Evento não encontrado.");

            var instrumentosDisponiveis = eventoService.GetInstrumentosDisponiveis(id);
            var minhaInscricao = await eventoService.GetSolicitacaoAssociado(id, idPessoa);

            var model = new EventoDetalhesAssociadoDTO
            {
                Id = evento.Id,
                DataHoraInicio = evento.DataHoraInicio,
                DataHoraFim = evento.DataHoraFim,
                Local = evento.Local,
                Repertorio = evento.Repertorio,
                InstrumentosDisponiveis = instrumentosDisponiveis,
                MinhaInscricao = minhaInscricao,
                // Lógica de permissão para os botões do Mobile
                PodeInscrever = minhaInscricao == null || minhaInscricao.Status == "NAO_SOLICITADO",
                PodeCancelar = minhaInscricao?.Status == "INSCRITO"
            };

            return Ok(model);
        }

        // POST: api/Evento/ResponderPresenca
        [HttpPost("ResponderPresenca")]
        public async Task<ActionResult> ResponderPresenca([FromBody] SolicitarParticipacaoDTO dto)
        {
            // Pega o ID do usuário logado via Token
            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);

            if (!await eventoService.PodeAssociadoSolicitar(dto.IdEvento, idPessoa))
            {
                return BadRequest("Não é possível solicitar participação neste evento.");
            }

            var resultado = await eventoService.SolicitarParticipacao(
                dto.IdEvento,
                idPessoa,
                dto.IdTipoInstrumento
            );

            if (resultado == HttpStatusCode.OK)
                return Ok(new { mensagem = "Presença confirmada!" });

            if (resultado == HttpStatusCode.Conflict)
                return Conflict(new { mensagem = "Você já solicitou este instrumento ou o status não permite." });

            return StatusCode(500, "Erro ao processar sua resposta.");
        }

        // POST: api/Evento/CancelarPresenca
        [HttpPost("CancelarPresenca/{idEvento}")]
        public async Task<ActionResult> CancelarPresenca(int idEvento)
        {
            int idPessoa = Convert.ToInt32(User.FindFirst("Id")?.Value);

            var resultado = await eventoService.CancelarSolicitacao(idEvento, idPessoa);

            if (resultado == HttpStatusCode.OK)
                return Ok(new { mensagem = "Solicitação cancelada com sucesso." });

            if (resultado == HttpStatusCode.BadRequest)
                return BadRequest("Não é possível cancelar uma solicitação já aprovada.");

            return NotFound("Solicitação não encontrada.");
        }
    }
}