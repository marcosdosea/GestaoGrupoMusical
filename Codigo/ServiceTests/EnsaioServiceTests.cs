using Core.Service;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service;
using Core.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests
{
    [TestClass]
    public class EnsaioServiceTests
    {/*
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
            var ensaios = new List<Ensaio>
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
            _context.AddRange(ensaios);
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
            var resultList = _ensaio.GetAll();
            var listaEnsaio = resultList.GetAwaiter().GetResult();
            Assert.AreEqual(4, listaEnsaio.Count());

            var result = _ensaio.Get(4);
            var ensaio = result.GetAwaiter().GetResult();

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
            _ensaio.Delete(2);
            // Assert
            var listaEnsaios = _ensaio.GetAll().GetAwaiter().GetResult();
            Assert.AreEqual(2, listaEnsaios.Count());
            var ensaio = _ensaio.Get(2).GetAwaiter().GetResult();
            Assert.AreEqual(0, ensaio.Id);
            Assert.AreEqual(null, ensaio.Local);
        }

        [TestMethod()]
        public void EditTest()
        {
            var ensaio = _ensaio.Get(3).GetAwaiter().GetResult();
            ensaio.Id = 3;
                   
            ensaio.IdGrupoMusical = 1;
            ensaio.DataHoraInicio = new DateTime(2023, 10, 11, 8, 0, 0);
            ensaio.DataHoraFim = new DateTime(2023, 10, 11, 12, 0, 0);
            ensaio.Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se";
            ensaio.Repertorio = "Ensaio de dança";
            ensaio.IdColaboradorResponsavel = 1;
            ensaio.IdRegente = 1;
            ensaio.PresencaObrigatoria = 1;
            ensaio.Tipo = "Extra";

            //Assert
            Assert.AreEqual(3, ensaio.Id);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", ensaio.Local);
            Assert.AreEqual(new DateTime(2023, 10, 11, 8, 0, 0), ensaio.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 10, 11, 12, 0, 0), ensaio.DataHoraFim);
            Assert.AreEqual("Ensaio de dança", ensaio.Repertorio);
            Assert.AreEqual(1, ensaio.IdGrupoMusical);
            Assert.AreEqual(1, ensaio.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaio.IdRegente);


        }
        [TestMethod()]
        public void GetTest()
        {
            var ensaio = _ensaio.Get(2).GetAwaiter().GetResult();
            Assert.AreEqual(2, ensaio.Id);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", ensaio.Local);
            Assert.AreEqual(new DateTime(2023, 9, 11, 8, 0, 0), ensaio.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 9, 11, 12, 0, 0), ensaio.DataHoraFim);
            Assert.AreEqual("Ensaio de bateria", ensaio.Repertorio);
            Assert.AreEqual(1, ensaio.IdGrupoMusical);
            Assert.AreEqual(1, ensaio.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaio.IdRegente);
            Assert.AreEqual("Extra", ensaio.Tipo);
            Assert.AreEqual(1, ensaio.PresencaObrigatoria);
          

        }
        [TestMethod()]
        public void GetAllTest()
        {
            var listaEnsaio = _ensaio.GetAll().GetAwaiter().GetResult();
            // Assert
            Assert.IsInstanceOfType(listaEnsaio, typeof(IEnumerable<Ensaio>));
            Assert.IsNotNull(listaEnsaio);
            Assert.AreEqual(3, listaEnsaio.Count());
            Assert.AreEqual(1, listaEnsaio.First().Id);
        }
        [TestMethod()]
        public void GetAllDTO()
        {
            // Act
            var listaEnsaio = _ensaio.GetAllDTO().GetAwaiter().GetResult();


            //Assert
            Assert.IsInstanceOfType(listaEnsaio, typeof(IEnumerable<EnsaioDTO>));
            Assert.IsNotNull(listaEnsaio);
            Assert.AreEqual(3, listaEnsaio.Count());
            Assert.AreEqual(0, listaEnsaio.First().Id);

        }
        */
    }

}
