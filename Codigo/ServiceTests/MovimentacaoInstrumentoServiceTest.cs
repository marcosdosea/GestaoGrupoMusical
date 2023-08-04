using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;


namespace Service.Tests
{
    [TestClass]
    public class MovimentacaoInstrumentoServiceTest
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoInstrumentoService _movimentacaoInstrumento;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical");
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var movimentacoesInstrumentos = new List<Movimentacaoinstrumento>
            {
                new Movimentacaoinstrumento
                {
                    Id = 1,
                    Data = new DateTime(2022, 8, 3),
                    IdInstrumentoMusical = 1,
                    IdAssociado = 1,
                    IdColaborador = 1,
                    ConfirmacaoAssociado = 0,
                    TipoMovimento = "EMPRESTIMO",
                },
                new Movimentacaoinstrumento
                {
                    Id = 2,
                    Data = new DateTime(2022, 8, 25),
                    IdInstrumentoMusical = 2,
                    IdAssociado = 2,
                    IdColaborador = 1,
                    ConfirmacaoAssociado = 1,
                    TipoMovimento = "DEVOLUCAO"
                },
                new Movimentacaoinstrumento
                {
                    Id = 3,
                    Data = new DateTime(2023, 2, 28),
                    IdInstrumentoMusical = 3,
                    IdAssociado = 3,
                    IdColaborador = 1,
                    ConfirmacaoAssociado = 0,
                    TipoMovimento = "EMPRESTIMO"
                }
            };

            _context.AddRange(movimentacoesInstrumentos);

            var pessoas = new List<Pessoa>
                {
                    new Pessoa {
                        Id = 1,
                        Cpf = "007.587.624-02",
                        Nome = "José santos",
                        Sexo = "M",
                        Cep = "49520-111",
                        Rua = "Rua 10",
                        Bairro = "Centro",
                        Cidade = "Itabaina",
                        Estado = "SE",
                        DataNascimento = DateTime.Parse("01-02-1992"),
                        Telefone1 = "79998567896",
                        Telefone2 = "79998653284",
                        Email = "josests@gmail.com",
                        DataEntrada = DateTime.Parse("10-01-2020"),
                        DataSaida = DateTime.Parse("05-06-2022"),
                        MotivoSaida = "Não me acostumei com a cultura",
                        Ativo = 1,
                        IsentoPagamento = 1,
                        IdGrupoMusical = 1,
                        IdPapelGrupo = 1,
                        IdManequim = 1
                    },
                    new Pessoa {
                        Id = 2,
                        Cpf = "007.457.624-02",
                        Nome = "Matheus santos",
                        Sexo = "M",
                        Cep = "49520-111",
                        Rua = "Rua 10",
                        Bairro = "Centro",
                        Cidade = "Itabaina",
                        Estado = "SE",
                        DataNascimento = DateTime.Parse("01-02-1985"),
                        Telefone1 = "79956567896",
                        Telefone2 = "79998653654",
                        Email = "matheussts@gmail.com",
                        DataEntrada = DateTime.Parse("10-01-2020"),
                        DataSaida = DateTime.Parse("05-06-2022"),
                        MotivoSaida = "Não me acostumei com a cultura",
                        Ativo = 1,
                        IsentoPagamento = 1,
                        IdGrupoMusical = 1,
                        IdPapelGrupo = 1,
                        IdManequim = 1
                    },

                    new Pessoa {
                        Id = 3,
                        Cpf = "007.217.424-02",
                        Nome = "Douglas santos",
                        Sexo = "M",
                        Cep = "49520-111",
                        Rua = "Rua 10",
                        Bairro = "Centro",
                        Cidade = "Itabaina",
                        Estado = "SE",
                        DataNascimento = DateTime.Parse("01-02-1998"),
                        Telefone1 = "79998567896",
                        Telefone2 = "79998653284",
                        Email = "Douglassts@gmail.com",
                        DataEntrada = DateTime.Parse("10-01-2019"),
                        DataSaida = DateTime.Parse("05-06-2021"),
                        MotivoSaida = "Não me acostumei com a cultura",
                        Ativo = 1,
                        IsentoPagamento = 1,
                        IdGrupoMusical = 1,
                        IdPapelGrupo = 1,
                        IdManequim = 1
                    }
                };

            _context.AddRange(pessoas);

            _context.SaveChanges();

            _movimentacaoInstrumento = new MovimentacaoInstrumentoService(_context);
        }

        [TestMethod]
        public void CreateAsyncTest()
        {
            // Act
            var result = _movimentacaoInstrumento.CreateAsync(new Movimentacaoinstrumento
            {
                Id = 4,
                Data = new DateTime(2023, 3, 10),
                IdInstrumentoMusical = 4,
                IdAssociado = 4,
                IdColaborador = 2,
                ConfirmacaoAssociado = 0,
                TipoMovimento = "EMPRESTIMO"
            }).GetAwaiter().GetResult();

            // Assert
            var movimentacaoInstrumento = _context.Movimentacaoinstrumentos.Find(3);
            Assert.AreEqual(200, result);
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(2, movimentacaoInstrumento.IdInstrumentoMusical);
        }

        [TestMethod]
        public void DeleteAsyncTest()
        {
            // Act
            _movimentacaoInstrumento.DeleteAsync(2).Wait();

            // Assert
            var movimentacaoInstrumento = _context.Movimentacaoinstrumentos.FindAsync(2).Result;
            Assert.IsNotNull(movimentacaoInstrumento);
        }

        [TestMethod]    
        public void GetAllByIdInstrumentoTest()
        {
            // Act
            var instrumentos = _movimentacaoInstrumento.GetAllByIdInstrumento(1).GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(instrumentos);
            Assert.AreEqual(1, instrumentos.Count());
            var movimentacaoInstrumento = instrumentos.First();
            Assert.AreEqual(1, movimentacaoInstrumento.Id);
            Assert.AreEqual(1, movimentacaoInstrumento.IdInstrumento);
            Assert.AreEqual("007.587.624-02", movimentacaoInstrumento.Cpf);
            Assert.AreEqual("José santos", movimentacaoInstrumento.NomeAssociado);
            Assert.AreEqual(new DateTime(2022, 8, 3), movimentacaoInstrumento.Data);
            Assert.AreEqual("Empréstimo", movimentacaoInstrumento.Movimentacao);
            Assert.AreEqual("Aguardando Confirmação", movimentacaoInstrumento.Status);
        }

        [TestMethod]
        public void GetEmprestimoByIdInstrumentoTest()
        {
            // Act
            var movimentacaoInstrumento = _movimentacaoInstrumento.GetEmprestimoByIdInstrumento(3).GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(3,movimentacaoInstrumento.Id);
            Assert.AreEqual(3, movimentacaoInstrumento.IdInstrumentoMusical);
            Assert.AreEqual(new DateTime(2023, 2, 28), movimentacaoInstrumento.Data);
            Assert.AreEqual(3, movimentacaoInstrumento.IdAssociado);
            Assert.AreEqual(1, movimentacaoInstrumento.IdColaborador);
            Assert.AreEqual(0, movimentacaoInstrumento.ConfirmacaoAssociado);
            Assert.AreEqual("EMPRESTIMO", movimentacaoInstrumento.TipoMovimento);
        }
    }
}
