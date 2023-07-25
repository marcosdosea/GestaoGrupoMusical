using Core.DTO;

namespace Core.Service
{
    public interface IInstrumentoMusicalService
    {
        Task<int> Create(Instrumentomusical instrumentoMusical);
        Task<int> Edit(Instrumentomusical instrumentoMusical);
        Task Delete(int id);
        Task<Instrumentomusical?> Get(int id);
        Task<IEnumerable<Instrumentomusical>> GetAll();
        Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO(int idGrupo);
        Task<string> GetNomeInstrumento(int id);
        Task<IEnumerable<Tipoinstrumento>> GetAllTipoInstrumento();
    }
}
