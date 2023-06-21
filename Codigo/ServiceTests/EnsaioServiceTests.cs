using Core.Service;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Service;
using NuGet.Frameworks;
using Core.DTO;

namespace Service.Tests
{
    [TestClass]
    public class EnsaioServiceTests
    {
        private GrupoMusicalContext _context;
        private IEnsaioService _ensaio;
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
            var ensaio = new List<Ensaio>
            {
                new Ensaio
                {
                    Id = 1,
                    IdGrupoMusical = 1,
                    DataHoraInicio = new DateTime(2023, 8, 11, 8, 0, 0),
                    DataHoraFim = new DateTime(2023, 8, 11, 12, 0, 0),
                    Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                    Repertorio = "Ensaio de Percusão",
                    IdColaboradorResponsavel = 1,
                    IdRegente = 1,
                    PresencaObrigatoria = 1,
                    Tipo = "Extra",
                },
                new Ensaio
                {
                    Id = 2,
                    IdGrupoMusical = 1,
                    DataHoraInicio = new DateTime(2023, 9, 11, 8, 0, 0),
                    DataHoraFim = new DateTime(2023, 9, 11, 12, 0, 0),
                    Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                    Repertorio = "Ensaio de bateria",
                    IdColaboradorResponsavel = 1,
                    IdRegente = 1,
                    PresencaObrigatoria = 1,
                    Tipo = "Extra",
                },
                 new Ensaio
                {
                    Id = 3,
                    IdGrupoMusical = 1,
                    DataHoraInicio = new DateTime(2023, 10, 11, 8, 0, 0),
                    DataHoraFim = new DateTime(2023, 10, 11, 12, 0, 0),
                    Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                    Repertorio = "Ensaio de dança",
                    IdColaboradorResponsavel = 1,
                    IdRegente = 1,
                    PresencaObrigatoria = 1,
                    Tipo = "Extra",

                }


        };
            _context.AddRange(ensaio);
            _context.SaveChanges();

            _ensaio = new EnsaioService(_context);

        }
        [TestMethod()]
        public void CreateTest()
        {
            _ensaio.Create(new Ensaio
            {
                Id = 4,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 10, 11, 8, 0, 0),
                DataHoraFim = new DateTime(2023, 10, 14, 12, 0, 0),
                Local = "Centro batalá, rua: estancia, 180, centro, Aracaju-se",
                Repertorio = "Ensaio de Xilindro",
                IdColaboradorResponsavel = 1,
                IdRegente = 1,
                PresencaObrigatoria = 1,
                Tipo = "Extra",
            });
            Assert.AreEqual(4, _ensaio.GetAll().Count);
            var ensaio = _ensaio.Get(4);
            Assert.AreEqual(4, ensaio.Id);
            Assert.AreEqual("Centro batalá, rua: estancia, 180, centro, Aracaju-se", ensaio.Local);
            Assert.AreEqual(new DateTime(2023, 10, 11, 8, 0, 0), ensaio.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 10, 14, 12, 0, 0), ensaio.DataHoraFim);
            Assert.AreEqual("Ensaio de Xilindro", ensaio.Repertorio);
            Assert.AreEqual(1, ensaio.IdGrupoMusical);
            Assert.AreEqual(1, ensaio.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaio.IdRegente);


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
            var evento = _ensaio.Get(1);
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
            Assert.IsNotNull(listaEvento);
            Assert.AreEqual(3, listaEvento.Count());
            Assert.AreEqual(0, listaEvento.First().Id);

        }

    }

}
