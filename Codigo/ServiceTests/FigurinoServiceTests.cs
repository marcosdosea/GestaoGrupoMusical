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

        [TestMethod]
        public void DeleteTest()
        {
            // Act 
            _figurino.Delete(1);

            // Arrange
            var figurino = _context.Figurinos.FindAsync(1).Result;
            Assert.IsNull(figurino); 
        }

        [TestMethod]
        public void EditTest()
        {
            // Act
            var figurino = _figurino.Get(2).Result;
            figurino.Nome = "Flash";
            figurino.Data = new DateTime(2022, 8, 21, 0, 0, 0, 0, DateTimeKind.Local);
            _figurino.Edit(figurino);

            // Assert
            var figurinoEditado = _figurino.Get(2).Result;
            Assert.IsNotNull(figurinoEditado);
            Assert.AreEqual(2, figurinoEditado.Id);
            Assert.AreEqual("Flash", figurinoEditado.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 21, 0, 0, 0, 0, DateTimeKind.Local), figurinoEditado.Data);
        }

        [TestMethod]
        public void GetTest()
        {
            // Act
            var figurino = _figurino.Get(3).Result;

            // Assert
            Assert.IsNotNull(figurino);
            Assert.AreEqual(3, figurino.Id);
            Assert.AreEqual("Batman", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical) ;
        }
    }
}
