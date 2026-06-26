using Core.DTO;
namespace Core.Service
{
    public interface IDispositivoService
    {
        Task EnviarNotificacaoParaGrupoAsync(int idGrupoMusical, string titulo, string corpo);
        Task<bool> RegistrarDispositivoAsync(RegistrarDispositivoDto dto);
    }
}