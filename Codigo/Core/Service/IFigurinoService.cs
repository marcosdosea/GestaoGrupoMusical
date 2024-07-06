using Core.Datatables;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IFigurinoService
    {
        Task<HttpStatusCode> Create(Figurino figurino);
        Task<HttpStatusCode> Edit(Figurino figurino);
        Task<HttpStatusCode> Delete(int id);

        Task<Figurino> Get(int id);

        /// <summary>
        /// Retorna uma lista contendo todos os figurinos
        /// naquele grupo musical
        /// </summary>
        /// <param name="cpf">cpf da pessoa autenticada para poder filtrar os figurinos</param>
        /// <returns>lista contendo todos os figurinos</returns>
        Task<IEnumerable<Figurino>> GetAll(int idGrupo);
        Task<IEnumerable<FigurinoDropdownDTO>> GetAllFigurinoDropdown(int idGrupo);

        Task<Figurino> GetByName(string name);
        Task<IEnumerable<EstoqueDTO>> GetAllEstoqueDTO(int id);

        /// <summary>
        /// Cria um estoque de um figurino
        /// </summary>
        /// <param name="estoque">entidade do estoque contendo id de figurino, manequim e quantidade que será disponibilizada</param>
        /// <returns>
        /// 200: estoque criado
        /// 201: estoque atualizada
        /// 400: falta algum dos id's
        /// 401: nao existe quantidade para disponibilizar
        /// 500: nao conseguiu criar o estoque ou atualizar
        /// </returns>
        Task<HttpStatusCode> CreateEstoque(Figurinomanequim estoque);
        Task<HttpStatusCode> DeleteEstoque(int idFigurino, int idManequim);
        Task<HttpStatusCode> EditEstoque(Figurinomanequim estoque);
        Task<EstoqueDTO> GetEstoque(int idFigurino, int idManequim);

        Task<DatatableResponse<Figurino>> GetDataPage(DatatableRequest request, int idGrupo);
    }
}
