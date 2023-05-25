namespace Core.Service
{
    public interface IApresentacao
    {
        int Create(Apresentacao apresentacao);
        void Edit(Apresentacao apresentacao);
        void Delete(int id);
        Apresentacao Get(int id);
        IEnumerable<Apresentacao> GetAll();
    }
}
