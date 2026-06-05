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
                var dispositivoExistente = await context.DispositivoPessoas
                    .FirstOrDefaultAsync(d => d.IdPessoa == dto.IdPessoa);

                if (dispositivoExistente != null)
                {
                    dispositivoExistente.FcmToken = dto.FcmToken;
                    dispositivoExistente.DataAtualizacao = DateTime.Now;
                    context.DispositivoPessoas.Update(dispositivoExistente);
                }
                else
                {
                    var novoDispositivo = new DispositivoPessoas
                    {
                        IdPessoa = dto.IdPessoa,
                        FcmToken = dto.FcmToken,
                        DataAtualizacao = DateTime.Now
                    };
                    await context.DispositivoPessoas.AddAsync(novoDispositivo);
                }
                await context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task EnviarNotificacaoParaGrupoAsync(int idGrupo, string titulo, string corpo)
        {
            var tokens = await context.DispositivoPessoas
                .Where(d => d.Pessoa.IdGrupoMusical == idGrupo)
                .Select(d => d.FcmToken)
                .ToListAsync();

            foreach (var token in tokens)
            {
                await notificacaoAdminService.Enviar(token, titulo, corpo);
            }
        }
    }
}