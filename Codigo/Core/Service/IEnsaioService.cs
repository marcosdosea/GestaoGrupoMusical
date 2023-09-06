using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IEnsaioService
    {
        /// <summary>
        /// Cadastra uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// BadRequest - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// PreconditionFailed - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Create(Ensaio ensaio, IEnumerable<int> idRegentes);

        /// <summary>
        /// Editar uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// BadRequest - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// PreconditionFailed - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Edit(Ensaio ensaio);

        /// <summary>
        /// Excluir uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Delete(int id);
        Task<Ensaio> Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();
        Task<EnsaioFrequenciaDTO?> GetFrequenciaAsync(int idEnsaio, int idGrupoMusical);

        /// <summary>
        /// Registra a lista de frequência no banco de dados
        /// </summary>
        /// <param name="frequencias"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// BadRequest - Lista vazia <para />
        /// Conflict - Lista enviada não corresponde a lista existente <para />
        /// NotFound - Lista não encontrada <para />
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> RegistrarFrequenciaAsync(List<EnsaioListaFrequenciaDTO> frequencias);
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO(int idGrupo);
        EnsaioDetailsDTO GetDetailsDTO(int idEnsaio);
        Task<IEnumerable<EnsaioAssociadoDTO>> GetEnsaiosByIdPesoaAsync(int idPessoa);
        Task<Ensaiopessoa?> GetEnsaioPessoaAsync(int idEnsaio, int idPessoa);

        /// <summary>
        /// Registra justificativa de ensaio
        /// </summary>
        /// <param name="frequencias"></param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso <para />
        /// HttpStatusCode.NotFound - Relação ensaio e pessoa não encontrada <para />
        /// HttpStatusCode.Unauthorized - Usuário não tem permissão para realizar a operação <para/>
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> RegistrarJustificativaAsync(int idEnsaio, int idPessoa, string? justificativa);
    }
}
