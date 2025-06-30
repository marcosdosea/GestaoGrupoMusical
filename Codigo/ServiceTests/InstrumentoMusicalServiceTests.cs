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
            //Arrange
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
        public async void CreateTest()
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
            Assert.AreEqual(DateTime.Parse("18/12/2018"), result.DataAquisicao);
            Assert.AreEqual("EMPRESTADO", result.Status);
            Assert.AreEqual(9, result.IdTipoInstrumento);
            Assert.AreEqual(7, result.IdGrupoMusical);
        }

        [TestMethod()]
        public async void DeleteTest()
        {
            // Act
            await _instrumentoMusical.Delete(2);

            // Assert
            var listaInstrumentoMusicais = await _instrumentoMusical.GetAll();
            Assert.AreEqual(2, listaInstrumentoMusicais.Count());
            var instrumentoMusical = await _instrumentoMusical.Get(2);
            Assert.AreEqual(null, instrumentoMusical);
        }

        [TestMethod()]
        public async void EditTest()
        {
            //Act
            var instrumentoMusical = await _instrumentoMusical.Get(3);
            instrumentoMusical.Id = 7;
            instrumentoMusical.Patrimonio = "7";
            instrumentoMusical.DataAquisicao = DateTime.ParseExact("24/02/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture); 
            instrumentoMusical.Status = "DISPONIVEL";
            instrumentoMusical.IdTipoInstrumento = 7;
            instrumentoMusical.IdGrupoMusical = 0;

            //Arrange
            Assert.AreEqual(7, instrumentoMusical.Id);
            Assert.AreEqual("7", instrumentoMusical.Patrimonio);
            Assert.AreEqual(DateTime.Parse("24/02/2020"), instrumentoMusical.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", instrumentoMusical.Status);
            Assert.AreEqual(7, instrumentoMusical.IdTipoInstrumento);
            Assert.AreEqual(0, instrumentoMusical.IdGrupoMusical);
        }

        [TestMethod()]
        public async void GetTest()
        {
            //Act
            var instrumentoMusical = await _instrumentoMusical.Get(1);

            //Arrange
            Assert.IsNotNull(instrumentoMusical);
            Assert.AreEqual(1, instrumentoMusical.Id);
            Assert.AreEqual("1", instrumentoMusical.Patrimonio);
            Assert.AreEqual(DateTime.Parse("24/02/13"), instrumentoMusical.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", instrumentoMusical.Status);
            Assert.AreEqual(0, instrumentoMusical.IdTipoInstrumento);
            Assert.AreEqual(0, instrumentoMusical.IdGrupoMusical);
        }

        [TestMethod()]
        public async void GetAllTest()
        {
            //Act
            var listaInstrumentoMusical = await _instrumentoMusical.GetAll();

            //Arrange
            Assert.IsInstanceOfType(listaInstrumentoMusical, typeof(IEnumerable<Instrumentomusical>));
            Assert.IsNotNull(listaInstrumentoMusical);
            Assert.AreEqual(3, listaInstrumentoMusical.Count());
            Assert.AreEqual(1, listaInstrumentoMusical.First().Id);
        }
    }
}
