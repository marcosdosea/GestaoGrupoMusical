using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests
{
    [TestClass]
    public class EventoServiceTests
    {/*
        private GrupoMusicalContext _context;
        private IEventoService _evento;
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
            var eventos = new List<Evento>
            {
                new Evento
                {
                    Id = 1,
                    IdGrupoMusical =1,
                    DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0),
                    DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0),
                    Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                    Repertorio = "Ensaio de Percusão",
                    IdColaboradorResponsavel = 1,
                    IdRegente = 1 
                },
                new Evento
                {
                    Id = 2,
                    IdGrupoMusical =1,
                    DataHoraInicio = new DateTime(2023, 6, 30, 14, 0, 0),
                    DataHoraFim = new DateTime(2023, 7, 30, 16, 0, 0),
                    Local = "Ginásio Professor Lima, rua: Francisco bragança, 260, centro, Aracaju-se",
                    Repertorio = "Recepcão de alunos novos",
                    IdColaboradorResponsavel = 2,
                    IdRegente = 1
                },
                 new Evento
                {
                    Id = 3,
                    IdGrupoMusical =2,
                    DataHoraInicio = new DateTime(2023, 7, 4, 18, 0, 0),
                    DataHoraFim = new DateTime(2023, 7, 4, 20, 30, 0),
                    Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                    Repertorio = "Ensaio de repique",
                    IdColaboradorResponsavel = 3,
                    IdRegente = 2
                }

            
        };
            _context.AddRange(eventos);
            _context.SaveChanges();

            _evento = new EventoService(_context);

        }
        [TestMethod()]
        public void CreateTest()
        {
            _evento.Create(new Evento
            {
                Id = 4,
                IdGrupoMusical = 2,
                DataHoraInicio = new DateTime(2023, 7, 5, 17, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 5, 18, 30, 0),
                Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                Repertorio = "Ensaio de Caixa",
                IdColaboradorResponsavel = 4,
                IdRegente = 2
            });
            Assert.AreEqual(4, _evento.GetAll().Count());
            var evento = _evento.Get(4);
            Assert.AreEqual(4, evento.Id);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", evento.Local);
            Assert.AreEqual(new DateTime(2023, 7, 5, 17, 0, 0), evento.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 5, 18, 30, 0), evento.DataHoraFim);
            Assert.AreEqual("Ensaio de Caixa", evento.Repertorio);
            Assert.AreEqual(2, evento.IdGrupoMusical);
            Assert.AreEqual(4, evento.IdColaboradorResponsavel);
            Assert.AreEqual(2, evento.IdRegente);


        }
        [TestMethod()]
        public void DeleteTest()
        {
            _evento.Delete(3);
            // Assert
            Assert.AreEqual(2, _evento.GetAll().Count());
            var evento = _evento.Get(3);
            Assert.AreEqual(null, evento);
        }

        [TestMethod()]
        public void EditTest()
        {
            var evento = _evento.Get(1);
            evento.Id = 1;
            evento.IdGrupoMusical = 1;
            evento.DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0);
            evento.DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0);
            evento.Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se";
            evento.Repertorio = "Ensaio de Repique";
            evento.IdColaboradorResponsavel = 1;
            evento.IdRegente = 1;

            //Assert
            Assert.AreEqual(1, evento.Id);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", evento.Local);
            Assert.AreEqual(new DateTime(2023, 7, 11, 8, 0, 0), evento.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 11, 12, 0, 0), evento.DataHoraFim);
            Assert.AreEqual("Ensaio de Repique", evento.Repertorio);
            Assert.AreEqual(1, evento.IdGrupoMusical);
            Assert.AreEqual(1, evento.IdColaboradorResponsavel);
            Assert.AreEqual(1, evento.IdRegente);


        }
        [TestMethod()]
        public void GetTest()
        {
            var evento = _evento.Get(2);
            Assert.AreEqual(2, evento.Id);
            Assert.AreEqual("Ginásio Professor Lima, rua: Francisco bragança, 260, centro, Aracaju-se", evento.Local);
            Assert.AreEqual(new DateTime(2023, 6, 30, 14, 0, 0), evento.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 30, 16, 0, 0), evento.DataHoraFim);
            Assert.AreEqual("Recepcão de alunos novos", evento.Repertorio);
            Assert.AreEqual(1, evento.IdGrupoMusical);
            Assert.AreEqual(2, evento.IdColaboradorResponsavel);
            Assert.AreEqual(1, evento.IdRegente);

        }
        [TestMethod()]
        public void GetAllTest()
        {
            var listaEvento = _evento.GetAll();
            // Assert
            Assert.IsInstanceOfType(listaEvento, typeof(IEnumerable<Evento>));
            Assert.IsNotNull(listaEvento);
            Assert.AreEqual(3, listaEvento.Count());
            Assert.AreEqual(1, listaEvento.First().Id);
        }
        [TestMethod()]
        public void GetAllDTO()
        {
            // Act
            var listaEvento = _evento.GetAllDTO();

            //Assert
            Assert.IsInstanceOfType(listaEvento, typeof(IEnumerable<EventoDTO>));
            Assert.IsNotNull (listaEvento);
            Assert.AreEqual(3,listaEvento.Count());
            Assert.AreEqual(0, listaEvento.First().Id);
            
        }
        */
    }

}
