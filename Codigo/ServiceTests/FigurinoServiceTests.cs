using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class FigurinoServiceTests
    {
        private GrupoMusicalContext _context;
        private IFigurinoService _figurino;

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

            var figurinos = new List<Figurino>
            {
                new Figurino
                {
                    Id = 1,
                    Nome = "Galinha Pintadinha",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 2,
                    Nome = "Mickey Mouse",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 3,
                    Nome = "Batman",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 4,
                    Nome = "Laterna Verde",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
            };

            _context.Figurinos.AddRange(figurinos);
            _context.SaveChanges();

            _figurino = new FigurinoService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            _figurino.Create(new Figurino
            {
                Id = 5,
                Nome = "Mulher Maravilha",
                Data = new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Local),
                IdGrupoMusical = 1
            }).Wait();

            // Assert
            var figurino = _figurino.Get(5).Result;
            Assert.IsNotNull(figurino);
            Assert.AreEqual(5, figurino.Id);
            Assert.AreEqual("Mulher Maravilha", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical);
        }
    }
}
