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
        /// Cadastra uma pessoa no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de entrada fora do escopo,ou seja, passa do dia atual<para />
        /// 401 - Data de nascimento fora do escopo, ou seja, ou passar do dia atual ou idade passa de 120 anos <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Create(Pessoa pessoa);

        /// <summary>
        /// Edita uma pessoa no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 400 - Data de entrada fora do escopo,ou seja, passa do dia atual<para />
        /// 401 - Data de nascimento fora do escopo, ou seja, ou passar do dia atual ou idade passa de 120 anos <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> Edit(Pessoa pessoa);
        void Delete(int id);
        Pessoa Get(int id);
        IEnumerable<Pessoa> GetAll();
        Task<IEnumerable<AssociadoDTO>> GetAllAssociadoDTO();

        bool GetCPFExistente(int id, string cpf);

        /// <summary>
        /// Cadastrar um associado ou não do grupo musical como  administrador do sistema
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Associado não existia, mas foi criado como administrador de grupo musical
        /// 201 - Associado existia e foi promovido
        /// 400 - O associado faz parte de outro grupo musical
        /// 401 - O associado já é um administrador daquele grupo musical
        /// 500 - O associado já possui cadastro em um grupo musical, não foi possivel alterar ele para adm grupo musical
        /// 501 - Erro na operação
        /// </returns>
        Task<int> AddAdmGroup(Pessoa pessoa);
        Task<IEnumerable<AdministradorGrupoMusicalDTO>> GetAllAdmGroup(int id);
        Task<bool> RemoveAdmGroup(int id);

        Task<bool> ToCollaborator(int id);
        Task<bool> RemoveCollaborator(int id);

        IEnumerable<Papelgrupo> GetAllPapelGrupo();

        /// <summary>
        ///  Mudar o campo de ativo "Sim" para "Não",isso é um maneira de dizer que associado tá deletado
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> RemoverAssociado(Pessoa pessoa, String? motivoSaida);

        Task<bool> NotificarCadastroAdmGrupoAsync(Pessoa pessoa);

        Task<UserDTO?> GetByCpf(string? cpf);

        /// <summary>
        /// Cadastra um Associado no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// 200 - Sucesso <para />
        /// 450 - Erro durante o cadastro identity <para />
        /// 500 - Erro interno
        /// </returns>
        Task<int> AddAssociadoAsync(Pessoa pessoa);

        Task<bool> NotificarCadastroAssociadoAsync(Pessoa pessoa);

        /// <summary>
        /// Retorna todos os associados que pertecem ao
        /// mesmo grupo musical da pessoal que está autenticado
        /// </summary>
        /// <param name="cpf">cpf da pessoa que está autentica</param>
        /// <returns>todas as pessoas que estão no mesmo grupo musical</returns>
        Task<IEnumerable<AssociadoDTO>> GetAllAssociadoDTOByGroup(String cpf);

        /// <summary>
        /// Pegar Pessoas de um grupo musical e em ordem
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>Uma lista de pessoas de um grupo em ordem alfabetica</returns>
        IEnumerable<Pessoa> GetAllPessoasOrder(int idGrupo);

        /// <summary>
        /// Gera senhas aleatorias
        /// </summary>
        /// <param name="length">tamanho da senha</param>
        /// <returns>senha</returns>
        Task<string> GenerateRandomPassword(int length);

        /// <summary>
        /// Embaralha a string passada para ele
        /// </summary>
        /// <param name="password">string a ser embaralhada</param>
        /// <returns>string embaralhada</returns>
        Task<string> PasswordShuffle(string password);
    }
}
