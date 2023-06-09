using Core;
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
            await _context.Pessoas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

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
                var pessoaF = await _context.Pessoas.FirstOrDefaultAsync(p => p.Cpf == pessoa.Cpf);

                if (pessoaF == null)
                {
                    Create(pessoa);
                }
                else
                {
                    //id para adm de grupo == 3
                    pessoaF.IdPapelGrupo = 3;
                    _context.Update(pessoaF);
                }
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IAsyncEnumerable<Pessoa> GetAllAdmGroup(int id)
        {
            var AdmGroupList = _context.Pessoas.Where(P => P.IdGrupoMusical == id && P.IdPapelGrupo == 3);

            return AdmGroupList.AsAsyncEnumerable();
        }

        public async Task<bool> RemoveAdmGroup(int id)
        {
            try
            {
                var pessoa = _context.Pessoas.Find(id);
                pessoa.IdPapelGrupo = 1;

                _context.Pessoas.Update(pessoa);

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
            

        }
    }
}
