using Core.DTO;
using System.Data;
using System.Net;
using Core.Datatables;

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
        Task<HttpStatusCode> Create(Ensaio ensaio, IEnumerable<int> idRegentes, int idFigurino);

        /// <summary>
        /// Editar uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// BadRequest - Data de inicio fora do escopo,ou seja, é menor que a data de hoje <para />
        /// PreconditionFailed - Data de inicio fora do escopo, ou seja, ou seja a data inicio passa da data fim do evento<para />
        /// NotFound - Ensaio não encontrado <para/> 
        /// InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Edit(Ensaio ensaio, IEnumerable<int> idRegentes);

        /// <summary>
        /// Excluir uma ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>
        /// OK - Sucesso <para />
        /// InternalServerError - Erro interno
        /// </returns>
        HttpStatusCode Delete(int id);
        Ensaio Get(int id);
        Task<IEnumerable<Ensaio>> GetAll();
        Task<IEnumerable<EnsaioDTO>> GetAllDTO();

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
        HttpStatusCode RegistrarFrequencia(FrequenciaEnsaioDTO frequencia, int quantidadeAssociados);
        Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO(int idGrupo);
        EnsaioDetailsDTO GetDetailsDTO(int idEnsaio);
        Task<IEnumerable<int>> GetIdRegentesEnsaioAsync(int idEnsaio);
        IEnumerable<EnsaioAssociadoDTO>? GetEnsaiosEventosByIdPessoa(int idPessoa);
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
        Task<HttpStatusCode> NotificarEnsaioViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idEnsaio);

        List<AssociadoDTO> GetAssociadoAtivos(int idEnsaio);

        Task<DatatableResponse<EnsaioIndexDTO>> GetDataPage(DatatableRequest request, int idGrupo);
    }
}
