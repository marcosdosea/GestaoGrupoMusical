using Core.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// HttpStatusCode.Created - Sucesso <para />
        /// HttpStatusCode.BadRequest - Data de entrada fora do escopo,ou seja, passa do dia atual<para />
        /// HttpStatusCode.NotAcceptable - Data de nascimento fora do escopo, ou seja, ou passar do dia atual ou idade passa de 120 anos <para />
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Create(Pessoa pessoa);

        /// <summary>
        /// Edita uma pessoa no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso <para />
        /// HttpStatusCode.BadRequest - Data de entrada fora do escopo,ou seja, passa do dia atual<para />
        /// HttpStatusCode.NotAcceptable - Data de nascimento fora do escopo, ou seja, ou passar do dia atual ou idade passa de 120 anos <para />
        /// HttpStatusCodeInternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> Edit(Pessoa pessoa);
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
        /// HttpStatusCode.Created - Associado não existia, mas foi criado como administrador de grupo musical
        /// HttpStatusCode.OK - Associado existia e foi promovido
        /// HttpStatusCode.NotAcceptable - O associado faz parte de outro grupo musical
        /// HttpStatusCode.BadRequest - O associado já é um administrador daquele grupo musical
        /// HttpStatusCode.InternalServerError - Erro na operação
        /// </returns>
        Task<HttpStatusCode> AddAdmGroup(Pessoa pessoa);
        Task<IEnumerable<AdministradorGrupoMusicalDTO>> GetAllAdmGroup(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HttpStatusCode.OK - Administrador removido com sucesso
        /// HttpStatusCode.NotFound - Administrador não encontrado
        /// HttpStatusCode.InternalServerError - Erro no servidor ou em alguma operação 
        ///  return HttpStatusCode.NotAcceptable - Não permitir remover o unico administrador do grupo
        /// </returns>
        Task<HttpStatusCode> RemoveAdmGroup(int id);

        Task<HttpStatusCode> ToCollaborator(int id, int idPapel);
        Task<HttpStatusCode> RemoveCollaborator(int id);

        IEnumerable<Papelgrupo> GetAllPapelGrupo();

        /// <summary>
        ///  Mudar o campo de ativo "Sim" para "Não",isso é um maneira de dizer que associado tá deletado
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso <para />
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> RemoverAssociado(Pessoa pessoa, String? motivoSaida);

        Task<bool> NotificarCadastroAdmGrupoAsync(Pessoa pessoa);

        Task<UserDTO?> GetByCpf(string? cpf);

        /// <summary>
        /// Cadastra um Associado no banco de dados
        /// </summary>
        /// <param name="pessoa"></param>
        /// <returns>
        /// HttpStatusCode.OK - Sucesso <para />
        /// HttpStatusCode.BadRequest - Erro durante o cadastro identity <para />
        /// HttpStatusCode.InternalServerError - Erro interno
        /// </returns>
        Task<HttpStatusCode> AddAssociadoAsync(Pessoa pessoa);

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

        /// <summary>
        /// Retorna o nome do associado passando o cpf
        /// </summary>
        /// <param name="cpf">cpf do associado</param>
        /// <returns>nome do associado</returns>
        Task<string> GetNomeAssociado(string cpf);

        Task<string> GetNomeAssociadoByEmail(string email);

        /// <summary>
        /// Verifica se associado existe pelo email
        /// afim de verificar se email ja esta em
        /// uso
        /// </summary>
        /// <param name="email">email para verificar</param>
        /// <returns>true: caso exista; false: caso nao exista</returns>
        Task<bool> AssociadoExist(string email);

        /// <summary>
        /// Ativa associado logo após redefinir senha
        /// </summary>
        /// <param name="email">cpf do associado</param>
        /// <returns>
        /// HttpStatusCode.NotFound: Associado não encontrado
        /// HttpStatusCode.Unauthorized: Associado desativado por adm de grupo musical
        /// HttpStatusCode.InternalServerError: Erro ao editar Associado
        /// HttpStatusCode.NotImplemented: Erro na operação
        /// </returns>
        Task<HttpStatusCode> AtivarAssociado(string cpf);

        Task<IEnumerable<AutoCompleteRegenteDTO>> GetRegentesForAutoCompleteAsync(int idGrupoMusical);
    }
}
