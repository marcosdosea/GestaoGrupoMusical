using Core.DTO;

namespace Core.Service
{
    public interface IInstrumentoMusicalService
    {
        int Create(Instrumentomusical instrumentoMusical);
        void Edit(Instrumentomusical instrumentoMusical);
        void Delete(int id);
        Instrumentomusical Get(int id);
        IEnumerable<Instrumentomusical> GetAll();
        Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO();
        //IReadOnlyCollection
    }
}
