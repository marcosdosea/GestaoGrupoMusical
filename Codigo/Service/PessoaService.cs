using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Email;
using System.Security.Cryptography;
using System.Text;
using System.Net;

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
        public async Task<HttpStatusCode> Create(Pessoa pessoa)
        {

                try
                {
                    pessoa.Cpf = pessoa.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
                    pessoa.Cep = pessoa.Cep.Replace("-", string.Empty);

                    await _context.Pessoas.AddAsync(pessoa);

                    if (pessoa.DataEntrada == null && pessoa.DataNascimento == null)
                    {//Mensagem de sucesso
                        await _context.SaveChangesAsync();
                        return HttpStatusCode.Created;
                    }
                    else if (pessoa.DataNascimento != null)
                    {
                        int idade = Math.Abs(pessoa.DataNascimento.Value.Year - DateTime.Now.Year);
                        if (pessoa.DataNascimento <= DateTime.Today && idade < 120)
                        {
                            if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Today)
                            {//mensagem de sucesso
                               
                                await _context.SaveChangesAsync();
                                return HttpStatusCode.Created;
                            }
                            else
                            {
                                // erro 400, data de entrada fora do escopo
                                return HttpStatusCode.BadRequest;
                            }
                        }
                        else
                        {
                            // erro 401, data de nascimento está fora do escopo
                            return HttpStatusCode.NotAcceptable;
                        }
                    }
                    else if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                    {
                        await _context.SaveChangesAsync();
                        return HttpStatusCode.Created;
                    }
                    else
                    {
                        // erro 400, data de entrada fora do escopo
                        return HttpStatusCode.BadRequest;
                    }
                }
                catch (Exception ex)
                {
                    //Aconteceu algum erro do servidor ou interno
                    return HttpStatusCode.InternalServerError;
                }

        }

        /// <summary>
        /// Metodo que atualiza os dados de uma pessoa/associado
        /// </summary>
        /// <param name="pessoa">dados do associado</param>
        public async Task<HttpStatusCode> Edit(Pessoa pessoa)
        {
            //Criar excecao para data de nascimento, etc
            try
            {
                pessoa.Cpf = pessoa.Cpf.Replace("-", string.Empty).Replace(".", string.Empty);
                pessoa.Cep = pessoa.Cep.Replace("-", string.Empty);

                var pessoaDb = await _context.Pessoas.Where(p => p.Id == pessoa.Id).AsNoTracking().SingleOrDefaultAsync();
                if (pessoaDb != null && pessoaDb.Cpf == pessoa.Cpf)
                {
                    pessoa.IdGrupoMusical = pessoaDb.IdGrupoMusical;
                    pessoa.IdPapelGrupo = pessoaDb.IdPapelGrupo;

                    _context.Pessoas.Update(pessoa);
                }
                if (pessoa.DataEntrada == null && pessoa.DataNascimento == null)
                {//Mensagem de sucesso
                    await _context.SaveChangesAsync();
                    return HttpStatusCode.OK;
                }
                else if (pessoa.DataNascimento != null)
                {
                    int idade = Math.Abs(pessoa.DataNascimento.Value.Year - DateTime.Now.Year);
                    if (pessoa.DataNascimento <= DateTime.Now && idade < 120)
                    {
                        if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                        {//mensagem de sucesso

                            await _context.SaveChangesAsync();
                            return HttpStatusCode.OK;
                        }
                        else
                        {
                            // erro 400, data de entrada fora do escopo
                            return HttpStatusCode.BadRequest;
                        }
                    }
                    else
                    {
                        // erro 401, data de nascimento está fora do escopo
                        return HttpStatusCode.NotAcceptable;
                    }
                }
                else if (pessoa.DataEntrada == null || pessoa.DataEntrada < DateTime.Now)
                {
                    await _context.SaveChangesAsync();
                    return HttpStatusCode.OK;
                }
                else
                {
                    // erro 400, data de entrada fora do escopo
                    return HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                //Aconteceu algum erro do servidor ou interno
                return HttpStatusCode.InternalServerError;
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

        public async Task<HttpStatusCode> AddAdmGroup(Pessoa pessoa)
        {
            using var transaction = _context.Database.BeginTransaction();
            HttpStatusCode sucesso; //será usada para o retorno 200/201 de sucesso

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

                    if(await Create(pessoa) != HttpStatusCode.Created)
                    {
                        await transaction.RollbackAsync();
                        return HttpStatusCode.NotAcceptable;//nao foi possivel criar a pessoa
                    }

                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, pessoa.Cpf, CancellationToken.None);

                    user.Email = pessoa.Email;

                    var result = await _userManager.CreateAsync(user, await GenerateRandomPassword(30));

                    if (result.Succeeded)
                    {
                        bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                        }

                        var userDb = await _userManager.FindByNameAsync(pessoa.Cpf);
                        await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR GRUPO");
                    }
                    sucesso = HttpStatusCode.Created; //usuario CRIADO como administrador de grupo musical
                }
                //caso exista, seja do mesmo grupo musical e não seja adm de grupo (3)
                else if (pessoaF.IdGrupoMusical == pessoa.IdGrupoMusical && pessoaF.IdPapelGrupo != 3)
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
                        await _userManager.RemoveFromRoleAsync(user, "ASSOCIADO");
                        await _userManager.AddToRoleAsync(user, "ADMINISTRADOR GRUPO");
                    }
                    //caso não user identity exista
                    else
                    {
                        user = CreateUser();

                        await _userStore.SetUserNameAsync(user, pessoaF.Cpf, CancellationToken.None);

                        user.Email = pessoaF.Email;

                        var result = await _userManager.CreateAsync(user, await GenerateRandomPassword(30));

                        if (result.Succeeded)
                        {
                            bool roleExists = await _roleManager.RoleExistsAsync("ADMINISTRADOR GRUPO");
                            if (!roleExists)
                            {
                                await _roleManager.CreateAsync(new IdentityRole("ADMINISTRADOR GRUPO"));
                            }

                            var userDb = await _userManager.FindByNameAsync(pessoaF.Cpf);
                            await _userManager.AddToRoleAsync(userDb, "ADMINISTRADOR GRUPO");
                        }
                    }

                    //id para adm de grupo == 3
                    pessoaF.IdPapelGrupo = 3;
                    try
                    {
                        _context.Pessoas.Update(pessoaF);
                        await _context.SaveChangesAsync();
                    }
                    catch 
                    {
                        await transaction.RollbackAsync();
                        return HttpStatusCode.NotAcceptable;//o usuario já possui cadastro em um grupo musical, não foi possivel alterar ele para adm grupo musical
                    }

                    sucesso = HttpStatusCode.OK; //usuario promovido a administrador de grupo musical
                }
               
                else if (pessoaF.IdGrupoMusical == pessoa.IdGrupoMusical && pessoaF.IdPapelGrupo == 3)
                {
                    return HttpStatusCode.BadRequest; // usuario já é administrador de grupo musical
                }
                else
                {
                    await transaction.RollbackAsync();
                    return HttpStatusCode.NotAcceptable;//erro 400, o usuario associado a outro grupo musical
                }

                await transaction.CommitAsync();
                return sucesso; //200 - usuario nao existia; 201 - usuario existia e foi promovido
            }
            catch
            {
                await transaction.RollbackAsync();
                return HttpStatusCode.InternalServerError;//erro 500, do servidor
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
                        await _userManager.AddToRoleAsync(user, "ASSOCIADO");

                        pessoa.IdPapelGrupo = 1;

                        _context.Pessoas.Update(pessoa);

                        await _context.SaveChangesAsync();
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<HttpStatusCode> AddAssociadoAsync(Pessoa pessoa)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                HttpStatusCode createResult = await Create(pessoa);
                if (createResult != HttpStatusCode.Created)
                {
                    await transaction.RollbackAsync();
                    return createResult;
                }

                var user = CreateUser();

                user.Email = pessoa.Email;

                await _userStore.SetUserNameAsync(user, pessoa.Cpf, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, await GenerateRandomPassword(30));

                if (result.Succeeded)
                {
                    bool roleExists = await _roleManager.RoleExistsAsync("ASSOCIADO");
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole("ASSOCIADO"));
                    }

                    var userDb = await _userManager.FindByNameAsync(pessoa.Cpf);
                    await _userManager.AddToRoleAsync(userDb, "ASSOCIADO");

                    await transaction.CommitAsync();
                    return HttpStatusCode.OK;
                }

                await transaction.RollbackAsync();
                return HttpStatusCode.BadRequest;
            }
            catch
            {
                await transaction.RollbackAsync();
                return HttpStatusCode.InternalServerError;
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
        public async Task<HttpStatusCode> ToCollaborator(int id, int idPapelGrupo)
        {
            var pessoa = Get(id);
            var user = await _userManager.FindByNameAsync(pessoa.Cpf);

            if (pessoa == null || user == null)
            {
                return HttpStatusCode.NotFound;
            }

            try
            {
                //promoção de role
                await ChangeUserRole(user, pessoa.IdPapelGrupo, idPapelGrupo);
                //==============

                pessoa.IdPapelGrupo = idPapelGrupo;

                _context.Update(pessoa);

                await _context.SaveChangesAsync();

                return HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> RemoveCollaborator(int id)
        {
            var pessoa = Get(id);

            if(pessoa == null)
            {
                return HttpStatusCode.NotFound;
            }


            //uma query pois pode ser que o id seja alterado futuramente
            var idPapel = _context.Papelgrupos
                .Where(p => p.Nome.ToUpper() == "ASSOCIADO")
                .Select(p => p.IdPapelGrupo)
                .First();

            //aqui ha uma comparacao com id de papel da pessoa
            //isso e para evitar que um adm de grupo seja
            //rebaixado a associado
            if (idPapel != null)
            {
                try
                {
                    pessoa.IdPapelGrupo = idPapel;

                    _context.Update(pessoa);
                    await _context.SaveChangesAsync();

                    return HttpStatusCode.OK;
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> RemoverAssociado(Pessoa pessoaAssociada, String? motivoSaida)
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
            cpf = cpf.Replace("-", string.Empty).Replace(".", string.Empty);
            var query = _context.Set<Pessoa>().AsNoTracking().FirstOrDefault(p => p.Id == id && p.Cpf == cpf);
            if (query != null)
            {
                return true;
            }
            return false;
        }

        public async Task<UserDTO?> GetByCpf(string? cpf)
        {
            var query = (from pessoa in _context.Pessoas
                        where pessoa.Cpf == cpf
                        select new UserDTO
                        {
                           Id = pessoa.Id,
                           Nome = pessoa.Nome,
                           Papel = pessoa.IdPapelGrupoNavigation.Nome,
                           Sexo = pessoa.Sexo,
                           Cep = pessoa.Cep,
                           Rua = pessoa.Rua,
                           Bairro = pessoa.Bairro,
                           Cidade = pessoa.Cidade,
                           Estado = pessoa.Estado,
                           DataNascimento = pessoa.DataNascimento,
                           Telefone1 = pessoa.Telefone1,
                           Telefone2 = pessoa.Telefone2,
                           Email = pessoa.Email,
                           Ativo = Convert.ToBoolean(pessoa.Ativo),
                           IdGrupoMusical = pessoa.IdGrupoMusical,
                           IdPapelGrupo = pessoa.IdPapelGrupo
                        }
                        ).FirstOrDefaultAsync();

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
        public IEnumerable<Pessoa> GetAllPessoasOrder(int idGrupo)
        {
            return _context.Pessoas.Where(g => g.IdGrupoMusical == idGrupo
                && g.IdPapelGrupoNavigation.Nome == "Associado" && g.Ativo == 1)
                .OrderBy(g => g.Nome).AsNoTracking();
        }

        public async Task<string> GenerateRandomPassword(int length)
        {
            const string caracteresPermitidos = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ123456789!@#$%^&*()-_=+[]{}|;:,.<>?";
            const int minimoCaracteresEspeciais = 1;
            const int minimoNumeros = 1;

            StringBuilder senha = new StringBuilder();

            using (var rng = new RNGCryptoServiceProvider())
            {
                // Adicionar pelo menos um caractere especial
                byte[] randomBytes = new byte[1];

                rng.GetBytes(randomBytes);

                int indiceCaractereEspecial = randomBytes[0] % 20; // Índice entre 0 e 19

                senha.Append(caracteresPermitidos[indiceCaractereEspecial]);

                // Adicionar pelo menos um número
                rng.GetBytes(randomBytes);

                int indiceNumero = 20 + randomBytes[0] % 10; // Índice entre 20 e 29

                senha.Append(caracteresPermitidos[indiceNumero]);

                // Completar o restante da senha com caracteres aleatórios
                for (int i = 0; i < length - minimoCaracteresEspeciais - minimoNumeros; i++)
                {
                    rng.GetBytes(randomBytes);
                    int indiceCaractere = randomBytes[0] % caracteresPermitidos.Length;
                    senha.Append(caracteresPermitidos[indiceCaractere]);
                }
            }

            // Embaralhar a senha para torná-la mais segura
            string senhaEmbaralhada = await PasswordShuffle(senha.ToString());

            return senhaEmbaralhada;
        }

        public async Task<string> PasswordShuffle(string password)
        {
            char[] array = password.ToCharArray();
            Random rng = new Random();
            int n = array.Length;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }

            return new string(array);
        }

        public async Task<string> GetNomeAssociado(string cpf)
        {
            var pessoa = await GetByCpf(cpf);

            return pessoa.Nome;
        }

        public async Task<string> GetNomeAssociadoByEmail(string email)
        {
            var pessoaF = await (from pessoa in _context.Pessoas
                         where pessoa.Email == email
                         select pessoa).FirstOrDefaultAsync();

            return pessoaF.Nome;
        }

        public async Task<bool> AssociadoExist(string email)
        {
            var pessoa = await _context.Pessoas.Where(att => att.Email == email).AsNoTracking().SingleOrDefaultAsync();

            if (pessoa != null)
            {
                return true;
            }
            else
            {
                return false;
            }
               
        }

        public async Task<HttpStatusCode> AtivarAssociado(string cpf)
        {
            try
            {
                var pessoa = await _context.Pessoas.Where(p => p.Cpf == cpf).AsNoTracking().SingleOrDefaultAsync();

                //verificar se existe o associado
                if(pessoa != null)
                {
                    //se o associado não foi desativado por algum adm de grupo
                    if (String.IsNullOrEmpty(pessoa.DataSaida.ToString()) && String.IsNullOrEmpty(pessoa.MotivoSaida))
                    {
                        pessoa.Ativo = 1;

                        if (await Edit(pessoa) != HttpStatusCode.OK)
                        {
                            return HttpStatusCode.NotFound;
                        }

                        return HttpStatusCode.OK;
                    }
                    else
                    {
                        return HttpStatusCode.Unauthorized;
                    }  
                    //===========================================================//
                }
                else
                {
                    return HttpStatusCode.NotImplemented;
                }
                //=================================//
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<IEnumerable<AutoCompleteRegenteDTO>> GetRegentesForAutoCompleteAsync(int idGrupoMusical)
        {
            var query = _context.Pessoas
                        .Where(p => p.IdGrupoMusical == idGrupoMusical && p.IdPapelGrupo == 5)
                        .Select(p => new AutoCompleteRegenteDTO { Id = p.Id, Nome = p.Nome });

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<HttpStatusCode> ChangeUserRole(UsuarioIdentity user, int idRoleAtual, int idRrolePromo)
        {
            try
            {
                string papelAnterior = (await _context.Papelgrupos.FindAsync(idRoleAtual)).Nome.ToUpper();
                string papelPromoNome = (await _context.Papelgrupos.FindAsync(idRrolePromo)).Nome.ToUpper();

                bool roleExists = await _roleManager.RoleExistsAsync(papelPromoNome);

                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(papelPromoNome));
                }
                await _userManager.RemoveFromRoleAsync(user, papelAnterior);

                await _userManager.AddToRoleAsync(user, papelPromoNome);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.OK;
        }
    }
}
