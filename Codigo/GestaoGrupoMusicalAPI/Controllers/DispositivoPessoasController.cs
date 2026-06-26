using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoGrupoMusicalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DispositivoPessoaController : ControllerBase
    {
        private readonly IDispositivoService service;
        private readonly GrupoMusicalContext context;
        private readonly INotificacaoAdminService notificacaoAdminService;

        public DispositivoPessoaController(
            IDispositivoService service,
            GrupoMusicalContext context,
            INotificacaoAdminService notificacaoAdminService)
        {
            this.service = service;
            this.context = context;
            this.notificacaoAdminService = notificacaoAdminService;
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarDispositivoDto dto)
        {
            if (dto == null || dto.IdPessoa <= 0 || string.IsNullOrEmpty(dto.FcmToken))
            {
                return BadRequest("Dados inválidos. ID e Token são obrigatórios.");
            }

            bool sucesso = await service.RegistrarDispositivoAsync(dto);

            if (sucesso)
            {
                return Ok(new { mensagem = "Token do Firebase registrado com sucesso!" });
            }

            return StatusCode(500, "Ocorreu um erro interno ao salvar o dispositivo.");
        }

        // Endpoint para testar o gatilho de notificações em massa para um grupo
        [HttpPost("TestarDisparoParaGrupo/{idGrupo}")]
        public async Task<IActionResult> TestarDisparo(int idGrupo, [FromBody] string mensagem)
        {
            // 1. Busca todos os tokens de dispositivos dos membros deste grupo
            var tokens = await context.DispositivoPessoa
                .Where(d => d.Pessoa.IdGrupoMusical == idGrupo)
                .Select(d => d.FcmToken)
                .ToListAsync();

            if (!tokens.Any())
                return NotFound($"Nenhum dispositivo encontrado para o grupo {idGrupo}.");

            // 2. Dispara a notificação real via Firebase Admin SDK
            foreach (var token in tokens)
            {
                await notificacaoAdminService.Enviar(token, "Teste de Evento", mensagem);
            }

            return Ok($"Notificação enviada com sucesso para {tokens.Count} dispositivos.");
        }
    }
}