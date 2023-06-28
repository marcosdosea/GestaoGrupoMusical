using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly GrupoMusicalContext _context;

        public PessoaService(GrupoMusicalContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Metodo que cria uma nova entidade pessoa/associado
        /// </summary>
        /// <param name="pessoa">dados do novo associado</param>
        /// <returns>retorna o id referente a nova entidade criada</returns>
        public async Task<int> Create(Pessoa pessoa)
        {
            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();

            return pessoa.Id;
        }

        /// <summary>
        /// Metodo que atualiza os dados de uma pessoa/associado
        /// </summary>
        /// <param name="pessoa">dados do associado</param>
        public void Edit(Pessoa pessoa)
        {
            //Criar excecao para data de nascimento, etc
            _context.Update(pessoa);
            _context.SaveChanges();
        }

        /// <summary>
        /// Metodo que remove/deleta uma pessoa/associado
        /// </summary>
        /// <param name="id">id do alvo a ser deletado/removido</param>
        public void Delete(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            _context.Remove(pessoa);
            _context.SaveChanges();
        }

        /// <summary>
        /// Metodo que obtem informacoes de uma pessoa/associado pelo
        /// id
        /// </summary>
        /// <param name="id">id do alvo</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Pessoa Get(int id)
        {
            return _context.Pessoas.Find(id);
        }

        /// <summary>
        /// Metodo que retorna todos as pessoas cadastradas no sistema
        /// </summary>
        /// <returns>lista com todas as pessoas</returns>
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas.AsNoTracking();
        }

        public async Task<bool> AddAdmGroup(Pessoa pessoa)
        {
            try
            {
                //faz uma consulta para tentar buscar a primeira pessoa com o cpf que foi digitado
                var pessoaF = _context.Pessoas.FirstOrDefault(p => p.Cpf == pessoa.Cpf);

                if (pessoaF == null)
                {
                    pessoa.IdManequim = 1;
                    pessoa.IdPapelGrupo = 3;
                    pessoa.Ativo = 1;
                    pessoa.Cep = "";
                    pessoa.Estado = "";
                    pessoa.IsentoPagamento = 1;
                    pessoa.Telefone1 = "";

                    Create(pessoa);
                }
                else if (pessoaF.IdGrupoMusical == pessoa.IdGrupoMusical)
                {
                    //id para adm de grupo == 3
                    pessoaF.IdPapelGrupo = 3;
                    Edit(pessoaF);
                }
                else
                {
                    return false;
                }
                

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Este metodo recebe o id de um grupo musical e retorna
        /// um DTO de todos os adm daquele grupo
        /// </summary>
        /// <param name="id">id do grupo musical</param>
        /// <returns>lista de DTO contendo todos os adm do grupo</returns>
        public IAsyncEnumerable<AdministradorGrupoMusicalDTO> GetAllAdmGroup(int id)
        {
            var AdmGroupList = from pessoa in _context.Pessoas
                               where pessoa.IdGrupoMusical == id && pessoa.IdPapelGrupo == 3
                               select new AdministradorGrupoMusicalDTO
                               {
                                   Id = pessoa.Id,
                                   Nome = pessoa.Nome,
                                   Cpf = pessoa.Cpf,
                                   Email = pessoa.Email
                               };

            return AdmGroupList.AsAsyncEnumerable();
        }

        public async Task<bool> RemoveAdmGroup(int id)
        {
            try
            {
                var pessoa = _context.Pessoas.Find(id);
                pessoa.IdPapelGrupo = 1;

                _context.Pessoas.Update(pessoa);

                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
            

        }

        public IEnumerable<AssociadoDTO> GetAllAssociadoDTO()
        {
            return from pessoa in _context.Pessoas
                   select new AssociadoDTO
                   {
                       Id = pessoa.Id,
                       Nome = pessoa.Nome,
                       Ativo = pessoa.Ativo
                   };
        }
        public IEnumerable<Papelgrupo> GetAllPapelGrupo()
        {
            return _context.Papelgrupos.AsNoTracking();
        }

        /// <summary>
        /// Este metodo faz o update de uma pessoa para colaborador do grupo
        /// </summary>
        /// <param name="pessoa">objeto pessoa para fazer a mudança</param>
        /// <returns>retorna true caso de tudo certo e false caso nao de certo</returns>
        public async Task<bool> ToCollaborator(int id)
        {
            var pessoa = Get(id);

            //uma query pois pode ser que o id seja alterado futuramente
            var idPapel = _context.Papelgrupos
                .Where(p=>p.Nome == "Colaborador")
                .Select(p => p.IdPapelGrupo)
                .First();

            if (idPapel != null && idPapel.GetType() == typeof(int) && pessoa != null)
            {
                pessoa.IdPapelGrupo = idPapel;
                Edit(pessoa);
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> RemoveCollaborator(int id)
        {
            var pessoa = Get(id);

            //uma query pois pode ser que o id seja alterado futuramente
            var idPapel = _context.Papelgrupos
                .Where(p => p.Nome == "Associado")
                .Select(p => p.IdPapelGrupo)
                .First();

            //aqui ha uma comparacao com id de papel da pessoa
            //isso e para evitar que um adm de grupo seja
            //rebaixado a associado
            if (idPapel != null && idPapel.GetType() == typeof(int)
                && pessoa != null && pessoa.IdPapelGrupo <= (idPapel+1))
            {
                pessoa.IdPapelGrupo = idPapel;
                Edit(pessoa);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoverAssociado(Pessoa pessoa)
        {
            Edit(pessoa);
            
        }
    }
}
