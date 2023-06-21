using Core.Service;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

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
                    DataAquisicao = DateTime.Parse("24/02/2013"),
                    Status = "DISPONIVEL",
                    IdTipoInstrumento = 0 ,
                    IdGrupoMusical = 0
                },
                new Instrumentomusical
                {
                    Id = 2,
                    Patrimonio = "2",
                    DataAquisicao = DateTime.Parse("24/02/2013"),
                    Status = "EMPRESTADO",
                    IdTipoInstrumento = 1 ,
                    IdGrupoMusical = 0
                },
                new Instrumentomusical
                {
                    Id = 3,
                    Patrimonio = "3",
                    DataAquisicao = DateTime.Parse("24/02/2013"),
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
        public  void CreateTest()
        {
            // Act
            _instrumentoMusical.Create(
                new Instrumentomusical
                {
                    Id = 4,
                    Patrimonio = "4",
                    DataAquisicao = DateTime.Parse("18/12/2018"),
                    Status = "EMPRESTADO",
                    IdTipoInstrumento = 9,
                    IdGrupoMusical = 7
                });

            // Assert
            var resultList = _instrumentoMusical.GetAll();
            var listaInstrumentoMusicais = resultList.GetAwaiter().GetResult();
            Assert.AreEqual(4, listaInstrumentoMusicais.Count());

            var result = _instrumentoMusical.Get(4);
            var instrumentoMusical = result.GetAwaiter().GetResult();
            Assert.AreEqual(4, instrumentoMusical.Id);
            Assert.AreEqual("4", instrumentoMusical.Patrimonio);
            Assert.AreEqual(DateTime.Parse("18/12/2018"), instrumentoMusical.DataAquisicao);
            Assert.AreEqual("EMPRESTADO", instrumentoMusical.Status);
            Assert.AreEqual(9, instrumentoMusical.IdTipoInstrumento);
            Assert.AreEqual(7, instrumentoMusical.IdGrupoMusical);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            // Act
            _instrumentoMusical.Delete(2);

            // Assert
            var listaInstrumentoMusicais = _instrumentoMusical.GetAll().GetAwaiter().GetResult();
            Assert.AreEqual(2, listaInstrumentoMusicais.Count());
            var instrumentoMusical = _instrumentoMusical.Get(2).GetAwaiter().GetResult();
            Assert.AreEqual(null, instrumentoMusical);
        }

        [TestMethod()]
        public void EditTest()
        {
            //Act
            var instrumentoMusical = _instrumentoMusical.Get(3).GetAwaiter().GetResult();
            instrumentoMusical.Id = 7;
            instrumentoMusical.Patrimonio = "7";
            instrumentoMusical.DataAquisicao = DateTime.Parse("24/02/2020");
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
        public void GetTest()
        {
            //Act
            var instrumentoMusical = _instrumentoMusical.Get(1).GetAwaiter().GetResult();

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
        public void GetAllTest()
        {
            //Act
            var listaInstrumentoMusical = _instrumentoMusical.GetAll().GetAwaiter().GetResult();

            //Arrange
            Assert.IsInstanceOfType(listaInstrumentoMusical, typeof(IEnumerable<Instrumentomusical>));
            Assert.IsNotNull(listaInstrumentoMusical);
            Assert.AreEqual(3, listaInstrumentoMusical.Count());
            Assert.AreEqual(1, listaInstrumentoMusical.First().Id);
        }
    }
}
