using Core.DTO;
using System.Net;

namespace Core.Service
{
    public interface IInstrumentoMusicalService
    {
        ///<sumary>
        ///  Links para documentação dos codigos HttpStatus https://learn.microsoft.com/pt-br/dotnet/api/system.net.httpstatuscode?view=net-7.0 
        ///</sumary>
        ///
        /// <summary>
        /// Criar um instrumento no banco de dados
        /// </summary>
        /// <param name="instrumentoMusical">Objeto instrumento musical passado</param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso instrumento musical criado<para />
        /// HttpStatusCode.PreconditionFailed - Não passou na condicional, onde a data de aquisição tem que ser maior que o dia atual
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Create(Instrumentomusical instrumentoMusical);
        /// <summary>
        /// Editar um instrumento no banco de dados
        /// </summary>
        /// <param name="instrumentoMusical">Objeto instrumento musical passado</param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso instrumento musical editado<para />
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Edit(Instrumentomusical instrumentoMusical);
        /// <summary>
        /// Remove um instrumento no banco de dados
        /// </summary>
        /// <param name="id">Id do Instrumento</param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso instrumento musical deletado<para />
        /// HttpStatusCode.PreconditionFailed - Está associado a alguma devolução/empréstimo
        /// HttpStatusCode.NotFound - Instrumento não encontado <para />
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Delete(int id);
        Task<Instrumentomusical?> Get(int id);
        Task<IEnumerable<Instrumentomusical>> GetAll();
        Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO(int idGrupo);
        Task<string> GetNomeInstrumento(int id);
        Task<IEnumerable<Tipoinstrumento>> GetAllTipoInstrumento();
        Task<InstrumentoMusicalDeleteDTO> GetInstrumentoMusicalDeleteDTO(int id);
    }
}
