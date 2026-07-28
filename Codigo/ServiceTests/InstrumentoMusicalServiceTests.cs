using Core.Service;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Globalization;

namespace Service.Tests
{
    [TestClass()]
    public class InstrumentoMusicalServiceTests
    {
        private GrupoMusicalContext _context;
        private IInstrumentoMusicalService _instrumentoMusical;

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
            var instrumentosMusicais = new List<Instrumentomusical>
            {
                new Instrumentomusical
                {
                    Id = 1,
                    Patrimonio = "1",
                    DataAquisicao = DateTime.ParseExact("24/02/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Status = "DISPONIVEL",
                    IdTipoInstrumento = 0 ,
                    IdGrupoMusical = 0
                },
                new Instrumentomusical
                {
                    Id = 2,
                    Patrimonio = "2",
                    DataAquisicao = DateTime.ParseExact("24/02/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Status = "EMPRESTADO",
                    IdTipoInstrumento = 1 ,
                    IdGrupoMusical = 0
                },
                new Instrumentomusical
                {
                    Id = 3,
                    Patrimonio = "3",
                    DataAquisicao = DateTime.ParseExact("24/02/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Status = "DANIFICADO",
                    IdTipoInstrumento = 2 ,
                    IdGrupoMusical = 0
                }
            };

            _context.AddRange(instrumentosMusicais);
            _context.SaveChanges();

            _instrumentoMusical = new InstrumentoMusicalService(_context);
        }

        [TestMethod()]
        public async Task CreateTest() // Changed from async void to async Task
        {
            // Act
            await _instrumentoMusical.Create(
                new Instrumentomusical
                {
                    Id = 4,
                    Patrimonio = "4",
                    DataAquisicao = DateTime.ParseExact("18/12/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Status = "EMPRESTADO",
                    IdTipoInstrumento = 9,
                    IdGrupoMusical = 7
                });

            // Assert
            var resultList = await _instrumentoMusical.GetAll();
            Assert.AreEqual(4, resultList.Count());

            var result = await _instrumentoMusical.Get(4);
            Assert.AreEqual(4, result.Id);
            Assert.AreEqual("4", result.Patrimonio);
            Assert.AreEqual(DateTime.ParseExact("18/12/2018", "dd/MM/yyyy", CultureInfo.InvariantCulture), result.DataAquisicao); // Standardized parsing
            Assert.AreEqual("EMPRESTADO", result.Status);
            Assert.AreEqual(9, result.IdTipoInstrumento);
            Assert.AreEqual(7, result.IdGrupoMusical);
        }

        [TestMethod()]
        public async Task DeleteTest() // Changed from async void to async Task
        {
            // Act
            await _instrumentoMusical.Delete(2);

            // Assert
            var listaInstrumentoMusicais = await _instrumentoMusical.GetAll();
            Assert.AreEqual(2, listaInstrumentoMusicais.Count());
            var instrumentoMusical = await _instrumentoMusical.Get(2);
            Assert.IsNull(instrumentoMusical); // Simplified Assert.AreEqual(null, ...)
        }
        [TestMethod()]
        public async Task EditTest()
        {
            var instrumentoMusical = await _instrumentoMusical.Get(3);

            
            instrumentoMusical.Patrimonio = "7"; 
            instrumentoMusical.DataAquisicao = DateTime.ParseExact("24/02/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            instrumentoMusical.Status = "DISPONIVEL";
            instrumentoMusical.IdTipoInstrumento = 7;
            instrumentoMusical.IdGrupoMusical = 0;

            // Act
            await _instrumentoMusical.Edit(instrumentoMusical);
            var result = await _instrumentoMusical.Get(3); // Buscando o mesmo registro (Id 3) após a edição

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id); // O ID continua sendo 3
            Assert.AreEqual("7", result.Patrimonio);
            Assert.AreEqual(DateTime.ParseExact("24/02/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture), result.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", result.Status);
            Assert.AreEqual(7, result.IdTipoInstrumento);
            Assert.AreEqual(0, result.IdGrupoMusical);
        }

        [TestMethod()]
        public async Task GetTest() // Changed from async void to async Task
        {
            // Act
            var instrumentoMusical = await _instrumentoMusical.Get(1);

            // Assert
            Assert.IsNotNull(instrumentoMusical);
            Assert.AreEqual(1, instrumentoMusical.Id);
            Assert.AreEqual("1", instrumentoMusical.Patrimonio);
            Assert.AreEqual(DateTime.ParseExact("24/02/2013", "dd/MM/yyyy", CultureInfo.InvariantCulture), instrumentoMusical.DataAquisicao); // Fixed 2-digit year bug
            Assert.AreEqual("DISPONIVEL", instrumentoMusical.Status);
            Assert.AreEqual(0, instrumentoMusical.IdTipoInstrumento);
            Assert.AreEqual(0, instrumentoMusical.IdGrupoMusical);
        }

        [TestMethod()]
        public async Task GetAllTest() // Changed from async void to async Task
        {
            // Act
            var listaInstrumentoMusical = await _instrumentoMusical.GetAll();

            // Assert
            Assert.IsInstanceOfType(listaInstrumentoMusical, typeof(IEnumerable<Instrumentomusical>));
            Assert.IsNotNull(listaInstrumentoMusical);
            Assert.AreEqual(3, listaInstrumentoMusical.Count());
            Assert.AreEqual(1, listaInstrumentoMusical.First().Id);
        }
    }
}