using Core.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface IPessoaService
    {
        /// <summary>
        /// Cadastra uma movimentação no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de entrada fora do escopo,ou seja, passa do dia atual<para />
        /// 401 - Data de nascimento fora do escopo, ou seja, ou passar do dia atual ou idade passa de 120 anos <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Create(Pessoa pessoa);
        void Edit(Pessoa pessoa);
        void Delete(int id);
        Pessoa Get(int id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<AssociadoDTO> GetAllAssociadoDTO();

        bool GetCPFExistente(int id, string cpf);

        Task<bool> AddAdmGroup(Pessoa pessoa);
        Task<IEnumerable<AdministradorGrupoMusicalDTO>> GetAllAdmGroup(int id);
        Task<bool> RemoveAdmGroup(int id);

        Task<bool> ToCollaborator(int id);
        Task<bool> RemoveCollaborator(int id);

        IEnumerable<Papelgrupo> GetAllPapelGrupo();
        void RemoverAssociado(Pessoa pessoa, String? motivoSaida);

        Task<bool> NotificarCadastroAdmGrupoAsync(Pessoa pessoa);

    }
}
