using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestClass]
    public class MovimentacaoFigurinoServiceTests
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoFigurinoService _movimentacaoFigurino;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(warning => warning.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var movimentacoesFigurinos = new List<Movimentacaofigurino>
            {
                new Movimentacaofigurino
                {
                    Id = 1,
                    Data = new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 1,
                    IdAssociado = 1,
                    IdColaborador = 2,
                    Status = "DISPONIVEL",
                    ConfirmacaoRecebimento = 0 
                },
                new Movimentacaofigurino
                {
                    Id = 2,
                    Data = new DateTime(2023, 3, 28, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 2,
                    IdAssociado = 2,
                    IdColaborador = 2,
                    Status = "ENTREGUE",
                    ConfirmacaoRecebimento = 1
                },
                new Movimentacaofigurino
                {
                    Id = 3,
                    Data = new DateTime(2023, 4, 2, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 3,
                    IdAssociado = 3,
                    IdColaborador = 2,
                    Status = "DEVOLVIDO",
                    ConfirmacaoRecebimento = 1
                }
            };

            _context.AddRange(movimentacoesFigurinos);
            _context.SaveChanges();

            _movimentacaoFigurino = new MovimentacaoFigurinoService(_context);
        }

        [TestMethod]
        public void CreateAsyncTest()
        {
            // Act
            var result = _movimentacaoFigurino.CreateAsync(new Movimentacaofigurino
            {
                Id = 4,
                Data = new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Local),
                IdFigurino = 4,
                IdAssociado = 4,
                IdColaborador = 2,
                Status = "DISPONIVEL",
                ConfirmacaoRecebimento = 0
            }).Result;

            // Assert
            Assert.AreEqual(200, result);
            var movimentacaoFigurino = _context.Movimentacaofigurinos.FindAsync(4).Result;
            
            Assert.IsNotNull(movimentacaoFigurino);
            Assert.AreEqual(4, movimentacaoFigurino.Id);
        }
    }
}
