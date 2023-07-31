using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Task<int> Create (Grupomusical grupomusical);
        /// <summary>
        /// Metodo usado para editar um grupo musical
        /// </summary>
        /// <param name="grupomusical"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        Task<int> Edit(Grupomusical grupomusical);
        /// <summary>
        /// Metodo usado para deletar um grupo musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 caso seja sucesso ou 500 se ouver algum erro ao executar o metodo</returns>
        Task<int> Delete(int id);
        /// <summary>
        /// Pegar um Grupo Musical
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Retorna 1 grupo musical</returns>
        Grupomusical Get(int id);
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
        int GetIdGrupo(string cpf);
    }
}
