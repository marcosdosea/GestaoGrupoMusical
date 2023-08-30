using Core;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Service
{
    public class EnsaioService : IEnsaioService
    {
        private readonly GrupoMusicalContext _context;
        public EnsaioService(GrupoMusicalContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Cadastra um novo Ensaio no banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se cadastrou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<int> Create(Ensaio ensaio, IEnumerable<int> idRegentes)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if(ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        await _context.Ensaios.AddAsync(ensaio);

                        var associadosRegentes = _context.Pessoas
                                         .Where(p => (p.IdPapelGrupo == 1 || p.IdPapelGrupo == 5) && p.Ativo == 1 && p.IdGrupoMusical == ensaio.IdGrupoMusical)
                                         .Select(pessoa => new { pessoa.Id, pessoa.Email, pessoa.IdPapelGrupo }).AsNoTracking();

                        await _context.SaveChangesAsync();

                        EmailModel email = new()
                        {
                            Assunto = "Batalá - Novo Ensaio Cadastrado",
                            AddresseeName = "Associados e Regentes",
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                    $"<h3>Um novo ensaio foi cadastrado.</h3>\r\n" +
                                    "<div style=\"font-size: large;\">\r\n        " +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Início:</dt><dd>{ensaio.DataHoraInicio}</dd>" +
                                    $"<dt style=\"font-weight: 700;\">Data e Horário de Fim:</dt><dd>{ensaio.DataHoraFim}</dd>\n</div>"
                        };
                        
                        await associadosRegentes.ForEachAsync(async associadoRegente => {
                            Ensaiopessoa ensaioPessoa = new()
                            {
                                IdEnsaio = ensaio.Id,
                                IdPessoa = associadoRegente.Id,
                                Presente = 1
                            };
                            if(associadoRegente.IdPapelGrupo == 5 && idRegentes.Contains(associadoRegente.Id))
                            {
                                ensaioPessoa.IdPapelGrupoPapelGrupo = associadoRegente.IdPapelGrupo;
                                await _context.Ensaiopessoas.AddAsync(ensaioPessoa);
                                email.To.Add(associadoRegente.Email);
                            }
                            else
                            {
                                ensaioPessoa.IdPapelGrupoPapelGrupo = associadoRegente.IdPapelGrupo;
                                await _context.Ensaiopessoas.AddAsync(ensaioPessoa);
                                email.To.Add(associadoRegente.Email);
                            }
                        });

                        await EmailService.Enviar(email);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return 200;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return 400;
                    }
                   
                }
                else 
                {
                    await transaction.RollbackAsync();
                    return 401;
                }
             
            }
            catch
            {
                await transaction.RollbackAsync();
                return 500;
            }
        }
        /// <summary>
        /// Deleta um Ensaio do banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Verdadeiro(<see langword="true" />) se deletou com sucesso ou Falso(<see langword="false" />) se houve algum erro.</returns>
        public async Task<int> Delete(int id)
        {
            try
            {
                _context.Ensaios.Remove(await Get(id));
                await _context.SaveChangesAsync();
                return 200;
            }
            catch
            {
                return 500;
            }
        }
        /// <summary>
        /// Edita um Ensaio do banco de dados
        /// </summary>
        /// <param name="ensaio"></param>
        /// <returns>retorna um inteiro.</returns>
        public async Task<int> Edit(Ensaio ensaio)
        {

             try
             {
                var ensaioDb = await _context.Ensaios.Where(e => e.Id == ensaio.Id).AsNoTracking().SingleOrDefaultAsync();
                if(ensaioDb != null)
                {
                    ensaio.IdColaboradorResponsavel = ensaioDb.IdColaboradorResponsavel;
                    ensaio.IdGrupoMusical = ensaioDb.IdGrupoMusical;
                }
                _context.Ensaios.Update(ensaio);
                if (ensaio.DataHoraFim > ensaio.DataHoraInicio)
                {
                    if(ensaio.DataHoraInicio >= DateTime.Now)
                    {
                        await _context.SaveChangesAsync();
                        return 200;
                    }
                    else
                    {
                        return 400;
                    }
                   
                }
                else 
                {
                    return 401;
                }
             
             }
             catch
             {
                return 500;
             }
        }
        /// <summary>
        /// Consulta um Ensaio no banco de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns>O Ensaio correspondente ao id ou um Ensaio vazio</returns>
        public async Task<Ensaio> Get(int id)
        {
            return await _context.Ensaios.FindAsync(id) ?? new Ensaio();
        }
        /// <summary>
        /// Consulta todos os Ensaios no banco de dados
        /// </summary>
        /// <returns>Uma lista contendo todos os Ensaios</returns>
        public async Task<IEnumerable<Ensaio>> GetAll()
        {
            return await _context.Ensaios.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<EnsaioDTO>> GetAllDTO()
        {
            var query = _context.Ensaios
                .OrderBy(g => g.DataHoraInicio)
                .Select(g =>
                new EnsaioDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Local = g.Local
                }).AsNoTracking().ToListAsync();
            return await query;
        }

        public async Task<IEnumerable<EnsaioIndexDTO>> GetAllIndexDTO(int idGrupo)
        {
            var query = _context.Ensaios
                .OrderBy(g => g.DataHoraInicio)
                .Where(g => g.IdGrupoMusical ==  idGrupo)
                .Select(g => new EnsaioIndexDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    Tipo = g.Tipo,
                    Local = g.Local,
                    PresencaObrigatoria = g.PresencaObrigatoria

                }).AsNoTracking().ToListAsync();
            return await query;
        }

        public EnsaioDetailsDTO GetDetailsDTO(int idEnsaio)
        {
            /*var query = _context.Ensaios
                .Select(g => new EnsaioDetailsDTO
                {
                    Id = g.Id,
                    DataHoraInicio = g.DataHoraInicio,
                    DataHoraFim = g.DataHoraFim,
                    Tipo = g.Tipo,
                    Local = g.Local,
                    PresencaObrigatoria = g.PresencaObrigatoria == 1 ? "Sim" : "Não",
                    Repertorio = g.Repertorio,
                    //NomeRegente = g.IdRegenteNavigation.Nome,
                    IdGrupoMusical = g.IdGrupoMusical

                }).Where(g => g.Id == idEnsaio);

            return query.First();*/
            throw new NotImplementedException();
        }

        public async Task<EnsaioFrequenciaDTO?> GetFrequenciaAsync(int idEnsaio, int idGrupoMusical)
        {
            /*var query = from ensaio in _context.Ensaios
                        where ensaio.Id == idEnsaio && ensaio.IdGrupoMusical == idGrupoMusical
                        select new EnsaioFrequenciaDTO
                        {
                            Inicio = ensaio.DataHoraInicio,
                            Fim = ensaio.DataHoraFim,
                            //NomeRegnete = ensaio.IdRegenteNavigation.Nome,
                            Tipo = ensaio.Tipo,
                            Local = ensaio.Local,
                            Frequencias = _context.Ensaiopessoas
                            .Where(ensaioPessoa => ensaioPessoa.IdEnsaio == idEnsaio)
                            .OrderBy(ensaioPessoa => ensaioPessoa.IdPessoaNavigation.Nome)
                            .Select(ensaioPessoa => new EnsaioListaFrequenciaDTO
                            {
                                IdEnsaio = ensaioPessoa.IdEnsaio,
                                IdPessoa = ensaioPessoa.IdPessoa,
                                Cpf = ensaioPessoa.IdPessoaNavigation.Cpf,
                                NomeAssociado = ensaioPessoa.IdPessoaNavigation.Nome,
                                Justificativa = ensaioPessoa.JustificativaFalta,
                                Presente = Convert.ToBoolean(ensaioPessoa.Presente),
                                JustificativaAceita = Convert.ToBoolean(ensaioPessoa.JustificativaAceita),
                            }).AsEnumerable()
                        };

            return await query.AsNoTracking().SingleOrDefaultAsync();*/
            throw new NotImplementedException();
        }

        public async Task<int> RegistrarFrequenciaAsync(List<EnsaioListaFrequenciaDTO> frequencias)
        {
            try
            {
                if (!frequencias.Any())
                {
                    return 400;
                }
                int idEnsaio = frequencias.First().IdEnsaio;

                var dbFrequencias = _context.Ensaiopessoas
                                    .Where(ensaioPessoa => ensaioPessoa.IdEnsaio == frequencias.First().IdEnsaio)
                                    .OrderBy(ensaioPessoa => ensaioPessoa.IdPessoaNavigation.Nome);

                if (dbFrequencias == null)
                {
                    return 404;
                }

                if (dbFrequencias.Count() != frequencias.Count)
                {
                    return 401;
                }

                int pos = 0;
                await dbFrequencias.ForEachAsync(dbFrequencia =>
                {
                    if(dbFrequencia.IdEnsaio == frequencias[0].IdEnsaio && dbFrequencia.IdPessoa == frequencias[pos].IdPessoa)
                    {
                        dbFrequencia.JustificativaAceita = Convert.ToSByte(frequencias[pos].JustificativaAceita);
                        dbFrequencia.Presente = Convert.ToSByte(frequencias[pos].Presente);

                        _context.Update(dbFrequencia);
                    }
                    pos++;
                });

                await _context.SaveChangesAsync();

                return 200;
            }
            catch
            {
                return 500;
            }
        }
    }
}
