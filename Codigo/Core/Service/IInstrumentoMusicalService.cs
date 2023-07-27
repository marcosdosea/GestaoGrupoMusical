using Core.DTO;

namespace Core.Service
{
    public interface IInstrumentoMusicalService
    {
        Task<int> Create(Instrumentomusical instrumentoMusical);
        Task<int> Edit(Instrumentomusical instrumentoMusical);
        /// <summary>
        /// Remove um instrumento no banco de dados
        /// </summary>
        /// <param name="id">Id do Instrumento</param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 404 - Instrumento não encontado <para />
        /// 501 - Erro No Banco de Dados
        /// </returns>
        Task<int> Delete(int id);
        Task<Instrumentomusical?> Get(int id);
        Task<IEnumerable<Instrumentomusical>> GetAll();
        Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO(int idGrupo);
        Task<string> GetNomeInstrumento(int id);
        Task<IEnumerable<Tipoinstrumento>> GetAllTipoInstrumento();
    }
}
