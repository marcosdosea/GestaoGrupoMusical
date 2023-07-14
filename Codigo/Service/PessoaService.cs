using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Email;
using System.Diagnostics.Metrics;

namespace Service
{
    public class PessoaService : IPessoaService
    {
        private readonly GrupoMusicalContext _context;
        private readonly UserManager<UsuarioIdentity> _userManager;
        private readonly IUserStore<UsuarioIdentity> _userStore;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PessoaService(GrupoMusicalContext context,
                            UserManager<UsuarioIdentity> userManager,
                            IUserStore<UsuarioIdentity> userStore,
                            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Metodo que cria uma nova entidade pessoa/associado
        /// </summary>
        /// <param name="pessoa">dados do novo associado</param>
        /// <returns>retorna o id referente a nova entidade criada</returns>
        public async Task<int> Create(Pessoa pessoa)
        {

                try
                {
                    await _context.Pessoas.AddAsync(pessoa);
                    if (pessoa.DataEntrada == null && pessoa.DataNascimento == null)
                    {//Mensagem de sucesso
                        await _context.SaveChangesAsync();
                        return 200;
                    }
                    else if (pessoa.DataNascimento != null)
                    {
                        int idade = Math.Abs(pessoa.DataNascimento.Value.Year - DateTime.Now.Year);
                        if (pessoa.DataNascimento <= DateTime.Now && idade < 120)
                        {
                            if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                            {//mensagem de sucesso
                               
                                await _context.SaveChangesAsync();
                                return 200;
                            }
                            else
                            {
                                // erro 400, data de entrada fora do escopo
                                return 400;
                            }
                        }
                        else
                        {
                            // erro 401, data de nascimento está fora do escopo
                            return 401;
                        }
                    }
                    else if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                    {
                        await _context.SaveChangesAsync();
                        return 200;
                    }
                    else
                    {
                        // erro 400, data de entrada fora do escopo
                        return 400;
                    }
                }
                catch (Exception ex)
                {
                    //Aconteceu algum erro do servidor ou interno
                    return 500;
                }

        }

        /// <summary>
        /// Metodo que atualiza os dados de uma pessoa/associado
        /// </summary>
        /// <param name="pessoa">dados do associado</param>
        public async Task<int> Edit(Pessoa pessoa)
        {
            //Criar excecao para data de nascimento, etc
            try
            {
                _context.Pessoas.Update(pessoa);
                if (pessoa.DataEntrada == null && pessoa.DataNascimento == null)
                {//Mensagem de sucesso
                    await _context.SaveChangesAsync();
                    return 200;
                }
                else if (pessoa.DataNascimento != null)
                {
                    int idade = Math.Abs(pessoa.DataNascimento.Value.Year - DateTime.Now.Year);
                    if (pessoa.DataNascimento <= DateTime.Now && idade < 120)
                    {
                        if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                        {//mensagem de sucesso

                            await _context.SaveChangesAsync();
                            return 200;
                        }
                        else
                        {
                            // erro 400, data de entrada fora do escopo
                            return 400;
                        }
                    }
                    else
                    {
                        // erro 401, data de nascimento está fora do escopo
                        return 401;
                    }
                }
                else if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                {
                    await _context.SaveChangesAsync();
                    return 200;
                }
                else
                {
                    // erro 400, data de entrada fora do escopo
                    return 400;
                }
            }
            catch (Exception ex)
            {
                //Aconteceu algum erro do servidor ou interno
                return 500;
            }


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

                    await Create(pessoa);

                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, pessoa.Cpf, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, pessoa.Cpf);

                    if (result.Succeeded)
                    {
                        bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                        }

                        var userDb = await _userManager.FindByNameAsync(pessoa.Cpf);
                        await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR GRUPO");

                        await NotificarCadastroAdmGrupoAsync(pessoa);
                    }
                }
                else if (pessoaF.IdGrupoMusical == pessoa.IdGrupoMusical)
                {
                    var user = await _userManager.FindByNameAsync(pessoaF.Cpf);

                    if (user != null)
                    {
                        bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                        }
                        await _userManager.AddToRoleAsync(user, "ADMINISTRADOR GRUPO");

                        await NotificarCadastroAdmGrupoAsync(pessoaF);
                    }
                    else
                    {
                        user = CreateUser();

                        await _userStore.SetUserNameAsync(user, pessoaF.Cpf, CancellationToken.None);
                        var result = await _userManager.CreateAsync(user, pessoaF.Cpf);

                        if (result.Succeeded)
                        {
                            bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                            if (!roleExists)
                            {
                                await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                            }

                            var userDb = await _userManager.FindByNameAsync(pessoaF.Cpf);
                            await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR GRUPO");

                            await NotificarCadastroAdmGrupoAsync(pessoaF);
                        }
                    }

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
        public async Task<IEnumerable<AdministradorGrupoMusicalDTO>> GetAllAdmGroup(int id)
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

            return await AdmGroupList.ToListAsync();
        }

        public async Task<bool> RemoveAdmGroup(int id)
        {
            try
            {
                var pessoa = await _context.Pessoas.FindAsync(id);
                if (pessoa != null)
                {
                    var user = await _userManager.FindByNameAsync(pessoa.Cpf);
                    if (user != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, "ADMINISTRADOR GRUPO");
                    }
                    pessoa.IdPapelGrupo = 1;

                    _context.Pessoas.Update(pessoa);

                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }


        }

        public async Task<IEnumerable<AssociadoDTO>> GetAllAssociadoDTO()
        {
            var query = from pessoa in _context.Pessoas
                        where pessoa.IdPapelGrupo == 1
                        select new AssociadoDTO
                        {
                            Id = pessoa.Id,
                            Nome = pessoa.Nome,
                            Ativo = pessoa.Ativo
                        };

            return await query.AsNoTracking().ToListAsync();
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
                .Where(p => p.Nome == "Colaborador")
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
                && pessoa != null && pessoa.IdPapelGrupo <= (idPapel + 1))
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

        public void RemoverAssociado(Pessoa pessoaAssociada, String? motivoSaida)
        {
            pessoaAssociada.MotivoSaida = motivoSaida;
            pessoaAssociada.Ativo = 0;
            pessoaAssociada.DataSaida = DateTime.Now;
            Edit(pessoaAssociada);

        }

        public async Task<bool> NotificarCadastroAdmGrupoAsync(Pessoa pessoa)
        {
            try
            {

                EmailModel email = new()
                {
                    Assunto = "Batalá - Administrador do Grupo",
                    Body = "<div style=\"text-align: center;\">\r\n    " +
                    "<h1>Administrador do Grupo</h1>\r\n    " +
                    $"<h2>Olá, {pessoa.Nome}, a sua senha para acesso.</h2>\r\n" +
                    "<div style=\"font-size: large;\">\r\n        " +
                    $"<dt style=\"font-weight: 700;\">Login:</dt><dd>{pessoa.Cpf}</dd>" +
                    $"<dt style=\"font-weight: 700;\">Senha:</dt><dd>{pessoa.Cpf}</dd>"
                };

                email.To.Add(pessoa.Email);

                await EmailService.Enviar(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private UsuarioIdentity CreateUser()
        {
            try
            {
                return Activator.CreateInstance<UsuarioIdentity>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(UsuarioIdentity)}'. " +
                    $"Ensure that '{nameof(UsuarioIdentity)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public bool GetCPFExistente(int id, string cpf)
        {
            var query = _context.Set<Pessoa>().AsNoTracking().FirstOrDefault(p => p.Id == id && p.Cpf == cpf);
            if (query != null)
            {
                return true;
            }
            return false;
        }

        public async Task<Pessoa?> GetByCpf(string? cpf)
        {
            var query = (from pessoa in _context.Pessoas
                        where pessoa.Cpf == cpf
                        select pessoa).FirstOrDefaultAsync();

            return await query;
        }
    }
}
