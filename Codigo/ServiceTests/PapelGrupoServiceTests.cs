using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service.Tests
{
    [TestClass]
    public class PapelGrupoServiceTests
    {
        private GrupoMusicalContext _context;
        private IPapelGrupo _papelGrupo;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical");
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var PapeisGrupo = new List<Papelgrupo>
            {
                new Papelgrupo
                {
                    IdPapelGrupo = 1,
                    Nome = "COLABORADOR"
                },
                new Papelgrupo
                {
                    IdPapelGrupo = 2,
                    Nome = "ADMINISTRADOR GRUPO"
                },
                new Papelgrupo
                {
                    IdPapelGrupo = 3,
                    Nome = "ADMINISTRADOR SISTEMA"
                }
            };

            _context.AddRange(PapeisGrupo);
            _context.SaveChanges();

            _papelGrupo = new PapelGrupoService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            _papelGrupo.Create(new Papelgrupo
            {
                IdPapelGrupo = 4,
                Nome = "Maestro"
            });

            // Assert
            Assert.AreEqual(4, _papelGrupo.GetAll().Count());
            var papelGrupo = _papelGrupo.Get(4);
            Assert.AreEqual(4, papelGrupo.IdPapelGrupo);
            Assert.AreEqual("Maestro", papelGrupo.Nome);
        }

        [TestMethod]
        public void DeleteTest()
        {
            // Act
            _papelGrupo.Delete(3);

            // Assert
            Assert.AreEqual(2, _papelGrupo.GetAll().Count());
            var papelGrupo = _papelGrupo.Get(3);
            Assert.AreEqual(null, papelGrupo);
        }

        [TestMethod]
        public void EditTest()
        {
            // Act
            var papelGrupo = _papelGrupo.Get(1);
            papelGrupo.IdPapelGrupo = 1;
            papelGrupo.Nome = "Maestro";

            // Assert
            Assert.AreEqual(1, papelGrupo.IdPapelGrupo);
            Assert.AreEqual("Maestro", papelGrupo.Nome);
        }

        [TestMethod]
        public void GetTest()
        {
            // Act
            var papelGrupo = _papelGrupo.Get(1);

            // Arrange
            Assert.AreEqual(1, papelGrupo.IdPapelGrupo);
            Assert.AreEqual("COLABORADOR", papelGrupo.Nome);
        }

        [TestMethod]
        public void GetAllTest()
        {
            // Act
            var listaPapelGrupo = _papelGrupo.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaPapelGrupo, typeof(IEnumerable<Papelgrupo>));
            Assert.IsNotNull(listaPapelGrupo);
            Assert.AreEqual(3, listaPapelGrupo.Count());
            Assert.AreEqual(1, listaPapelGrupo.First().IdPapelGrupo);
        }

    }
}
