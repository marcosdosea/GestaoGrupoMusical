using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Email;

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
                    pessoa.Cpf = pessoa.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
                    pessoa.Cep = pessoa.Cep.Replace("-", string.Empty);

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
                pessoa.Cpf = pessoa.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
                pessoa.Cep = pessoa.Cep.Replace("-", string.Empty);

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

        public async Task<int> AddAdmGroup(Pessoa pessoa)
        {
            using var transaction = _context.Database.BeginTransaction();
            int sucesso; //será usada para o retorno 200/201 de sucesso

            try
            {
                pessoa.Cpf = pessoa.Cpf.Replace(".", String.Empty).Replace("-", String.Empty);
                //faz uma consulta para tentar buscar a primeira pessoa com o cpf que foi digitado
                var pessoaF = _context.Pessoas.FirstOrDefault(p => p.Cpf == pessoa.Cpf);

                //caso nao exista algeum com o cpf indicado
                if (pessoaF == null)
                {
                    pessoa.IdManequim = 1;
                    pessoa.IdPapelGrupo = 3;
                    pessoa.Ativo = 1;
                    pessoa.Cep = "";
                    pessoa.Estado = "";
                    pessoa.IsentoPagamento = 1;
                    pessoa.Telefone1 = "";

                    if(await Create(pessoa) != 200)
                    {
                        await transaction.RollbackAsync();
                        return 500;//nao foi possivel criar a pessoa
                    }

                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, pessoa.Cpf, CancellationToken.None);

                    user.Email = pessoa.Email;

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
                    sucesso = 200; //usuario CRIADO como administrador de grupo musical
                }
                //caso exista e seja do mesmo grupo musical
                else if (pessoaF.IdGrupoMusical == pessoa.IdGrupoMusical)
                {
                    var user = await _userManager.FindByNameAsync(pessoaF.Cpf);

                    //se o user identity existir
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
                    //caso não user identity exista
                    else
                    {
                        user = CreateUser();

                        await _userStore.SetUserNameAsync(user, pessoaF.Cpf, CancellationToken.None);

                        user.Email = pessoaF.Email;

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
                    if(await Edit(pessoaF) != 200)
                    {
                        await transaction.RollbackAsync();
                        return 500;//o usuario já possui cadastro em um grupo musical, não foi possiveç alterar ele para adm grupo musical
                    }

                    sucesso = 201; //usuario promovido a administrador de grupo musical
                }
                else
                {
                    await transaction.RollbackAsync();
                    return 400;//erro 400, o usuario associado a outro grupo musical
                }

                await transaction.CommitAsync();
                return sucesso;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 501;//erro 500, do servidor
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

        public async Task<int> AddAssociadoAsync(Pessoa pessoa)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                int createResult = await Create(pessoa);
                if (createResult != 200)
                {
                    await transaction.RollbackAsync();
                    return createResult;
                }

                var user = CreateUser();

                user.Email = pessoa.Email;

                await _userStore.SetUserNameAsync(user, pessoa.Cpf, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, pessoa.Cpf);

                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync("ASSOCIADO");
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ASSOCIADO"));
                    }

                    var userDb = await _userManager.FindByNameAsync(pessoa.Cpf);
                    await _userManager.AddToRoleAsync(userDb, "ASSOCIADO");

                    await NotificarCadastroAssociadoAsync(pessoa);

                    await transaction.CommitAsync();
                    return createResult;
                }

                await transaction.RollbackAsync();
                return 450;
            }
            catch
            {
                await transaction.RollbackAsync();
                return 500;
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

        public async Task<int> RemoverAssociado(Pessoa pessoaAssociada, String? motivoSaida)
        {
            pessoaAssociada.MotivoSaida = motivoSaida;
            pessoaAssociada.Ativo = 0;
            pessoaAssociada.DataSaida = DateTime.Now;
            return await Edit(pessoaAssociada);
            

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

        public async Task<bool> NotificarCadastroAssociadoAsync(Pessoa pessoa)
        {
            try
            {

                EmailModel email = new()
                {
                    Assunto = "Batalá - Acesso como Associado",
                    Body = "<div style=\"text-align: center;\">\r\n    " +
                    "<h1>Associado</h1>\r\n    " +
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

        public async Task<IEnumerable<AssociadoDTO>> GetAllAssociadoDTOByGroup(string cpf)
        {
            if (cpf == null)
            {
                return null;
            }

            int idGrupo;

            try
            {
                var pessoa = await GetByCpf(cpf);
                idGrupo = pessoa.IdGrupoMusical;
            }
            catch
            {
                return null;
            }

            var query = from pessoaQ in _context.Pessoas
                        where pessoaQ.IdGrupoMusical == idGrupo
                        && pessoaQ.IdPapelGrupo == 1            //retorna apenas associados
                        select new AssociadoDTO
                        {
                            Id = pessoaQ.Id,
                            Nome = pessoaQ.Nome,
                            Ativo = pessoaQ.Ativo
                        };

            return query;
        }
    }
}
