using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MateriaEstudoService.Tests
{
    [TestClass]
    public class MaterialEstudoServiceTests
    {
        private GrupoMusicalContext _context;
        private IMaterialEstudoService _materialEstudoService;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var materialestudos = new List<Materialestudo>
            {
                new Materialestudo
                {
                    Id = 1,
                    Nome = "Quadrilha",
                    Link = "www.drive.google.com",
                    Data = new DateTime(2023, 7, 11, 8, 0, 0),
                    IdGrupoMusical = 1,
                    IdColaborador = 1
                },
                new Materialestudo
                {
                    Id = 2,
                    Nome = "Frevo",
                    Link = "www.youtube.com",
                    Data = new DateTime(2023, 6, 30, 14, 0, 0),
                    IdGrupoMusical = 2,
                    IdColaborador = 1
                },
                new Materialestudo
                {
                    Id = 3,
                    Nome = "Samba de roda",
                    Link = "www.youtube.com",
                    Data = new DateTime(2021, 3, 15, 10, 0, 0),
                    IdGrupoMusical = 1,
                    IdColaborador = 3
                }
            };
            _context.AddRange(materialestudos);
            _context.SaveChanges();

            _materialEstudoService = new MaterialEstudoService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            var newMaterial = new Materialestudo
            {
                Id = 4,
                Nome = "Maracatu",
                Link = "www.youtube.com",
                Data = new DateTime(2023, 7, 11, 8, 0, 0),
                IdGrupoMusical = 1,
                IdColaborador = 1
            };

            _materialEstudoService.Create(newMaterial);

            // Assert
            var materialEstudo = _materialEstudoService.Get(4).Result;
            Assert.IsNotNull(materialEstudo);
            Assert.AreEqual(4, materialEstudo.Id);
            Assert.AreEqual("Maracatu", materialEstudo.Nome);
            Assert.AreEqual("www.youtube.com", materialEstudo.Link);
            Assert.AreEqual(new DateTime(2023, 7, 11, 8, 0, 0), materialEstudo.Data);
            Assert.AreEqual(1, materialEstudo.IdGrupoMusical);
            Assert.AreEqual(1, materialEstudo.IdColaborador);
        }
        [TestMethod]
        public void DeleteTest()
        {
            var materialEstudo = _context.Materialestudos.Find(1);
            Assert.IsNotNull(materialEstudo, "MaterialEstudo não encontrado para deletar.");

            _context.Materialestudos.Remove(materialEstudo);
            _context.SaveChanges();

            var deletedMaterialEstudo = _context.Materialestudos.Find(1);
            Assert.IsNull(deletedMaterialEstudo, "MaterialEstudo não foi deletado.");

        }
        [TestMethod]
        public void EditTest()
        {
            // Arrange
            var materialEstudo = _context.Materialestudos.Find(2);
            materialEstudo.Nome = "Xaxado";
            materialEstudo.Link = "www.youtube.com";
            materialEstudo.Data = (new DateTime(2021, 3, 15, 10, 0, 0));
            materialEstudo.IdGrupoMusical = 2;
            materialEstudo.IdColaborador = 1;

            Assert.IsNotNull(materialEstudo);
            Assert.AreEqual(2, materialEstudo.Id);
            Assert.AreEqual("Xaxado", materialEstudo.Nome);
            Assert.AreEqual("www.youtube.com", materialEstudo.Link);
            Assert.AreEqual(new DateTime(2021, 3, 15, 10, 0, 0), materialEstudo.Data);
            Assert.AreEqual(2, materialEstudo.IdGrupoMusical);
            Assert.AreEqual(1, materialEstudo.IdColaborador);

        }
        [TestMethod]
        public void GetTest()
        {
            var materialEstudo = _materialEstudoService.Get(2).Result;
            Assert.IsNotNull(materialEstudo);
            Assert.AreEqual(2, materialEstudo.Id);
            Assert.AreEqual("Frevo", materialEstudo.Nome);
            Assert.AreEqual("www.youtube.com", materialEstudo.Link);
            Assert.AreEqual(new DateTime(2023, 6, 30, 14, 0, 0), materialEstudo.Data);
            Assert.AreEqual(2, materialEstudo.IdGrupoMusical);
            Assert.AreEqual(1, materialEstudo.IdColaborador);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var materialEstudos = _materialEstudoService.GetAll().Result;
            Assert.IsNotNull(materialEstudos);
            Assert.AreEqual(3, materialEstudos.Count());
        }
        [TestMethod]
        public void GetAllMaterialEstudoIdGrupoTest()
        {
            var materialEstudos = _materialEstudoService.GetAllMaterialEstudoPerIdGrupo(2).Result;
            Assert.IsNotNull(materialEstudos);
            Assert.AreEqual(1, materialEstudos.Count());
        }
    }
}
