using Core.DTO;

namespace Core.Service
{
    public interface IEnsaioService
    {
        /// <summary>
        /// Cadastra uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// 401 - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Create(Ensaio ensaio);
        /// <summary>
        /// Editar uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// 401 - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Edit(Ensaio ensaio);
        /// <summary>
        /// Excluir uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Delete(int id);
        Task<Ensaio> Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();
        Task<EnsaioFrequenciaDTO?> GetFrequenciaAsync(int idEnsaio, int idGrupoMusical);
        /// <summary>
        /// Registra a lista de frequência no banco de dados
        /// </summary>
        /// <param name="frequencias"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Lista vazia <para />
        /// 401 - Lista enviada não corresponde a lista existente <para />
        /// 404 - Lista não encontrada <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> RegistrarFrequenciaAsync(List<EnsaioListaFrequenciaDTO> frequencias);
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO(int idGrupo);
        EnsaioDetailsDTO GetDetailsDTO(int idEnsaio);
        Task<IEnumerable<EnsaioAssociadoDTO>> GetEnsaiosByIdPesoaAsync(int idPessoa);
        Task<Ensaiopessoa?> GetEnsaioPessoaAsync(int idEnsaio, int idPessoa);
        Task<int> RegistrarJustificativaAsync(int idEnsaio, int idPessoa, string? justificativa);
    }
}
