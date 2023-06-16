using Core.DTO;

namespace Core.Service
{
    public interface IInstrumentoMusicalService
    {
        Task<int> Create(Instrumentomusical instrumentoMusical);
        Task Edit(Instrumentomusical instrumentoMusical);
        Task Delete(int id);
        Task<Instrumentomusical> Get(int id);
        Task<IEnumerable<Instrumentomusical>> GetAll();
        Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO();
        Task<string> GetNomeInstrumento(int id);
    }
}
