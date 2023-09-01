using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IGrupoMusicalService
    {
        /// <summary>
        /// Metodo usado para adicionar o Grupo Musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        Task<HttpStatusCode> Create (Grupomusical grupomusical);
        /// <summary>
        /// Metodo usado para editar um grupo musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        Task<HttpStatusCode> Edit(Grupomusical grupomusical);
        /// <summary>
        /// Metodo usado para deletar um grupo musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        Task<HttpStatusCode> Delete(int id);

        /// <summary>
        /// Pegar um Grupo Musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Retorna 1 grupo musical</returns>
        Task<Grupomusical> Get(int id);

        /// <summary>
        /// Pega todos os grupos musicais
        /// </summary>
        /// <returns>Uma lista de grupo musicais</returns>
        IEnumerable<Grupomusical> GetAll();
        /// <summary>
        /// DTO de grupo musicais
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        IEnumerable<GrupoMusicalDTO> GetAllDTO();

        /// <summary>
        /// Informar se cnpj a esxiste no banco 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        bool GetCNPJExistente(int id, string cnpj);

        /// <summary>
        /// Busacar id do Grupo apartir da pessoas que esta logada 
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>Id do grupo</returns>
        Task<int> GetIdGrupo(string cpf);

        /// <summary>
        /// Retorna todas as pessoas que possuem um papel acima de associado
        /// dentro do grupo
        /// </summary>
        /// <param name="idGrupo">id do grupo alvo</param>
        /// <returns>lista com todas as pessoas filtradas</returns>
        Task<IEnumerable<ColaboradoresDTO>> GetAllColaboradores(int idGrupo);

        /// <summary>
        /// Retorna os papeis: Colaborador e Regente
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Papelgrupo>> GetPapeis();
    }
}
