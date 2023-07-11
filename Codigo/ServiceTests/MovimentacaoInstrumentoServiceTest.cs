using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;


namespace ServiceTests
{
    [TestClass]
    public class MovimentacaoInstrumentoServiceTest
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoInstrumentoService _movimentacaoInstrumento;

        [TestInitialize]
        private void Initialize()
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
                    ConfirmacaoAssociado = 1,
                    TipoMovimento = "EMPRESTIMO"
                },
                new Movimentacaoinstrumento
                {
                    Id = 2,
                    Data = new DateTime(2023, 7, 5),
                    IdInstrumentoMusical = 2,
                    IdAssociado = 2,
                    IdColaborador = 1,
                    ConfirmacaoAssociado = 2,
                    TipoMovimento = "DEVOLUCAO"
                },
                new Movimentacaoinstrumento
                {
                    Id = 3,
                    Data = new DateTime(2023, 2, 28),
                    IdInstrumentoMusical = 3,
                    IdAssociado = 3,
                    IdColaborador = 2,
                    ConfirmacaoAssociado = 1,
                    TipoMovimento = "EMPRESTIMO"
                }
            };

            _context.AddRange(movimentacoesInstrumentos);
            _context.SaveChanges();

            _movimentacaoInstrumento = new MovimentacaoInstrumentoService(_context);
        }
    }
}
