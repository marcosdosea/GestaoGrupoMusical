using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class DispositivoService : IDispositivoService
    {
        private readonly GrupoMusicalContext context;
        private readonly INotificacaoAdminService notificacaoAdminService;

        // Adicionamos o notificacaoAdminService aqui
        public DispositivoService(GrupoMusicalContext context, INotificacaoAdminService notificacaoAdminService)
        {
            this.context = context;
            this.notificacaoAdminService = notificacaoAdminService;
        }

        public async Task<bool> RegistrarDispositivoAsync(RegistrarDispositivoDto dto)
        {
            try
            {
                var dispositivoExistente =
                    await context.DispositivoPessoa
                        .FirstOrDefaultAsync(
                            d => d.FcmToken == dto.FcmToken);

                if (dispositivoExistente != null)
                {
                    // O token já existe: transfere o aparelho
                    // para a pessoa atualmente autenticada.
                    dispositivoExistente.IdPessoa = dto.IdPessoa;
                    dispositivoExistente.DataAtualizacao = DateTime.Now;
                }
                else
                {
                    await context.DispositivoPessoa.AddAsync(
                        new DispositivoPessoa
                        {
                            IdPessoa = dto.IdPessoa,
                            FcmToken = dto.FcmToken,
                            DataAtualizacao = DateTime.Now
                        });
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erro ao registrar dispositivo: {ex.Message}");

                return false;
            }
        }

        public async Task EnviarNotificacaoParaGrupoAsync(int idGrupo, string titulo, string corpo)
        {
            var tokens = await context.DispositivoPessoa
                .Where(d => d.Pessoa.IdGrupoMusical == idGrupo)
                .Select(d => d.FcmToken)
                .Distinct()
                .ToListAsync();

            foreach (var token in tokens)
            {
                await notificacaoAdminService.Enviar(token, titulo, corpo);
            }
        }
    }
}