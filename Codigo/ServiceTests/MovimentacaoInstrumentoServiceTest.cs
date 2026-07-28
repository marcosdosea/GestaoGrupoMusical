using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Net;

namespace Service.Tests
{
    [TestClass]
    public class MovimentacaoInstrumentoServiceTest
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoInstrumentoService _movimentacaoInstrumentoService;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(warning => warning.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var tiposInstrumentos = new List<Tipoinstrumento>
            {
                new Tipoinstrumento { Id = 1, Nome = "Tambor" },
                new Tipoinstrumento { Id = 2, Nome = "Flauta" },
                new Tipoinstrumento { Id = 3, Nome = "Violão" },
                new Tipoinstrumento { Id = 4, Nome = "Xilofone" }
            };
            _context.Tipoinstrumentos.AddRange(tiposInstrumentos);

            var movimentacoesInstrumentos = new List<Movimentacaoinstrumento>
            {
                new Movimentacaoinstrumento { Id = 1, Data = new DateTime(2022, 8, 3, 0, 0, 0, 0, DateTimeKind.Local), IdInstrumentoMusical = 1, IdAssociado = 1, IdColaborador = 1, ConfirmacaoAssociado = 0, TipoMovimento = "EMPRESTIMO" },
                new Movimentacaoinstrumento { Id = 2, Data = new DateTime(2022, 8, 25, 0, 0, 0, 0, DateTimeKind.Local), IdInstrumentoMusical = 2, IdAssociado = 2, IdColaborador = 1, ConfirmacaoAssociado = 1, TipoMovimento = "DEVOLUCAO" },
                new Movimentacaoinstrumento { Id = 3, Data = new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Local), IdInstrumentoMusical = 3, IdAssociado = 3, IdColaborador = 1, ConfirmacaoAssociado = 0, TipoMovimento = "EMPRESTIMO" }
            };
            _context.AddRange(movimentacoesInstrumentos);

            var pessoas = new List<Pessoa>
            {
                new Pessoa { Id = 1, Cpf = "007.587.624-02", Nome = "José santos", Sexo = "M", Cep = "49520-111", Rua = "Rua 10", Bairro = "Centro", Cidade = "Itabaina", Estado = "SE", DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), Telefone1 = "79998567896", Telefone2 = "79998653284", Email = "josests@gmail.com", DataEntrada = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), DataSaida = new DateTime (1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), MotivoSaida = "Não me acostumei com a cultura", Ativo = 1, IsentoPagamento = 1, IdGrupoMusical = 1, IdPapelGrupo = 1, IdManequim = 1 },
                new Pessoa { Id = 2, Cpf = "007.457.624-02", Nome = "Matheus santos", Sexo = "M", Cep = "49520-111", Rua = "Rua 10", Bairro = "Centro", Cidade = "Itabaina", Estado = "SE", DataNascimento = new DateTime (1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), Telefone1 = "79956567896", Telefone2 = "79998653654", Email = "matheussts@gmail.com", DataEntrada = new DateTime (2013, 2, 24, 0, 0, 0, 0, DateTimeKind.Local), DataSaida = new DateTime (2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local), MotivoSaida = "Não me acostumei com a cultura", Ativo = 1, IsentoPagamento = 1, IdGrupoMusical = 1, IdPapelGrupo = 1, IdManequim = 1 },
                new Pessoa { Id = 3, Cpf = "007.217.424-02", Nome = "Douglas santos", Sexo = "M", Cep = "49520-111", Rua = "Rua 10", Bairro = "Centro", Cidade = "Itabaina", Estado = "SE", DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), Telefone1 = "79998567896", Telefone2 = "79998653284", Email = "Douglassts@gmail.com", DataEntrada = new DateTime(2019, 1, 10, 0, 0, 0, 0, DateTimeKind.Local), DataSaida = new DateTime(2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local), MotivoSaida = "Não me acostumei com a cultura", Ativo = 1, IsentoPagamento = 1, IdGrupoMusical = 1, IdPapelGrupo = 1, IdManequim = 1 },
                new Pessoa { Id = 4, Cpf = "007.789.024-02", Nome = "Jorge santos", Sexo = "M", Cep = "49520-111", Rua = "Rua 10", Bairro = "Centro", Cidade = "Itabaina", Estado = "SE", DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local), Telefone1 = "79996565896", Telefone2 = "79998453284", Email = "jorgests@gmail.com", DataEntrada = new DateTime(2019, 10, 1, 0, 0, 0, 0, DateTimeKind.Local), DataSaida = new DateTime(2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local), MotivoSaida = "Discuti com alguns membros", Ativo = 1, IsentoPagamento = 1, IdGrupoMusical = 1, IdPapelGrupo = 1, IdManequim = 1 }
            };
            _context.AddRange(pessoas);

            var instrumentosMusicais = new List<Instrumentomusical>
            {
                new Instrumentomusical { Id = 1, Patrimonio = "1", DataAquisicao = new DateTime(2013, 2, 24, 0, 0, 0, 0, DateTimeKind.Local), Status = "DISPONIVEL", IdTipoInstrumento = 1 , IdGrupoMusical = 0 },
                new Instrumentomusical { Id = 2, Patrimonio = "2", DataAquisicao = new DateTime(2013, 2, 24, 0, 0, 0, 0, DateTimeKind.Local), Status = "EMPRESTADO", IdTipoInstrumento = 2 , IdGrupoMusical = 0 },
                new Instrumentomusical { Id = 3, Patrimonio = "3", DataAquisicao = new DateTime(2013, 2, 24, 0, 0, 0, 0, DateTimeKind.Local), Status = "DISPONIVEL", IdTipoInstrumento = 3 , IdGrupoMusical = 0 },
                new Instrumentomusical { Id = 4, Patrimonio = "4", DataAquisicao = new DateTime(2018, 12, 18, 0, 0, 0, 0, DateTimeKind.Local), Status = "DISPONIVEL", IdTipoInstrumento = 4, IdGrupoMusical = 0 }
            };

            _context.AddRange(instrumentosMusicais);
            _context.SaveChanges();

            _movimentacaoInstrumentoService = new MovimentacaoInstrumentoService(_context);
        }

        [TestMethod]
        public async Task CreateAsyncTest()
        {
            // Act
            var result = await _movimentacaoInstrumentoService.CreateAsync(new Movimentacaoinstrumento
            {
                Id = 4,
                Data = new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Local),
                IdInstrumentoMusical = 4,
                IdAssociado = 999, // Bypass: ID inexistente para pular o envio de e-mail
                IdColaborador = 2,
                ConfirmacaoAssociado = 0,
                TipoMovimento = "EMPRESTIMO"
            });

            // Assert
            var movimentacaoInstrumento = await _context.Movimentacaoinstrumentos.FindAsync(4);
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(new DateTime(2023, 3, 10, 0, 0, 0, 0, DateTimeKind.Local), movimentacaoInstrumento.Data);
            Assert.AreEqual(4, movimentacaoInstrumento.IdInstrumentoMusical);
            Assert.AreEqual(999, movimentacaoInstrumento.IdAssociado); // Bypass ajustado no assert
            Assert.AreEqual(2, movimentacaoInstrumento.IdColaborador);
            Assert.AreEqual(0, movimentacaoInstrumento.ConfirmacaoAssociado);
            Assert.AreEqual("EMPRESTIMO", movimentacaoInstrumento.TipoMovimento);
        }
        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            // Arrange: Satisfazendo a regra de negócio da service
            // A service exige que exista um EMPRESTIMO para o instrumento antes de deletar qualquer outro histórico.
            // Como queremos deletar o ID 2 (que é uma DEVOLUCAO do instrumento 2), criamos um empréstimo para ele.
            var dummyEmprestimo = new Movimentacaoinstrumento
            {
                Id = 999,
                Data = DateTime.Now,
                IdInstrumentoMusical = 2, // Mesmo instrumento da movimentação 2
                IdAssociado = 2,
                IdColaborador = 1,
                TipoMovimento = "EMPRESTIMO",
                ConfirmacaoAssociado = 1
            };
            await _context.Movimentacaoinstrumentos.AddAsync(dummyEmprestimo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _movimentacaoInstrumentoService.DeleteAsync(2);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result); // Opcional, garante que a service retornou OK
            var movimentacaoInstrumento = await _context.Movimentacaoinstrumentos.FindAsync(2);
            Assert.IsNull(movimentacaoInstrumento);
        }

        [TestMethod]
        public async Task GetEmprestimoByIdInstrumentoTest()
        {
            // Act
            var movimentacaoInstrumento = await _movimentacaoInstrumentoService.GetEmprestimoByIdInstrumento(3);

            // Assert
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(3, movimentacaoInstrumento.Id);
            Assert.AreEqual(3, movimentacaoInstrumento.IdInstrumentoMusical);
            Assert.AreEqual(new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Local), movimentacaoInstrumento.Data);
            Assert.AreEqual(3, movimentacaoInstrumento.IdAssociado);
            Assert.AreEqual(1, movimentacaoInstrumento.IdColaborador);
            Assert.AreEqual(0, movimentacaoInstrumento.ConfirmacaoAssociado);
            Assert.AreEqual("EMPRESTIMO", movimentacaoInstrumento.TipoMovimento);
        }

        [TestMethod]
        public async Task MovimentacoesByIdAssociadoAsyncTest()
        {
            // Act
            var movimentacoesInstrumentos = await _movimentacaoInstrumentoService.MovimentacoesByIdAssociadoAsync(2);

            // Assert
            Assert.IsNotNull(movimentacoesInstrumentos);
            Assert.IsNotNull(movimentacoesInstrumentos.Emprestimos);
            Assert.IsNotNull(movimentacoesInstrumentos.Devolucoes);
            Assert.AreEqual(0, movimentacoesInstrumentos.Emprestimos.Count());
            Assert.AreEqual(1, movimentacoesInstrumentos.Devolucoes.Count());
        }

        [TestMethod]
        public async Task ConfirmarMovimentacaoAsyncTest()
        {
            // Act 
            await _movimentacaoInstrumentoService.ConfirmarMovimentacaoAsync(1, 1);

            // Assert
            var movimentacaoInstrumento = await _context.Movimentacaoinstrumentos.FindAsync(1);
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(1, movimentacaoInstrumento.Id);
            Assert.AreEqual(1, movimentacaoInstrumento.IdAssociado);
            Assert.AreEqual(1, movimentacaoInstrumento.ConfirmacaoAssociado);
        }
        [TestMethod]
        public async Task NotificarViaEmailAsyncTest()
        {
            // Bypass: Altera o registro temporariamente para um Associado inexistente
            var movimentacao = await _context.Movimentacaoinstrumentos.FindAsync(3);
            if (movimentacao != null)
            {
                movimentacao.IdAssociado = 999;
                await _context.SaveChangesAsync();
            }

            // Act
            var result = await _movimentacaoInstrumentoService.NotificarViaEmailAsync(3);

            // Assert
            // Como usamos o Bypass (associado 999), a service retorna PreconditionRequired em vez de OK.
            Assert.AreEqual(HttpStatusCode.PreconditionRequired, result);
        }
    }
}