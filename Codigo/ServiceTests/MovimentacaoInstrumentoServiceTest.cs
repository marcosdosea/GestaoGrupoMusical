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
                    TipoMovimento = "EMPRESTIMO"
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
            var movimentacaoInstrumento = _context.Movimentacaoinstrumentos.FindAsync(4).GetAwaiter().GetResult();
            Assert.AreEqual(200, result);
            Assert.IsNotNull(movimentacaoInstrumento);
            Assert.AreEqual(2, movimentacaoInstrumento.Id);
        }

        [TestMethod]
        public void DeleteAsyncTest()
        {
            // Act
            _movimentacaoInstrumento.DeleteAsync(2);

            // Assert
            var movimentacaoInstrumento = _context.Movimentacaoinstrumentos.FindAsync(2).Result;
            Assert.IsNotNull(movimentacaoInstrumento);
        }

    }
}
