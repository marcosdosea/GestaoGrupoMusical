namespace Core.Service
{
    public interface IEnsaioService
    {
        Task<bool> Create(Ensaio ensaio);
        Task<bool> Edit(Ensaio ensaio);
        Task<bool> Delete(int id);
        Task<Ensaio> Get(int id);
        IAsyncEnumerable<Ensaio> GetAll();
    }
}
