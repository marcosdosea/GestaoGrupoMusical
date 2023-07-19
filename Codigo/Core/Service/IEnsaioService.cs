using Core.DTO;

namespace Core.Service
{
    public interface IEnsaioService
    {
        Task<int> Create(Ensaio ensaio);
        Task<int> Edit(Ensaio ensaio);
        Task<bool> Delete(int id);
        Task<Ensaio> Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO();
    }
}
