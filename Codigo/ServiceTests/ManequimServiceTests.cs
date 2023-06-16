using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests
{
    [TestClass]
    public class ManequimServiceTests
    {
        private GrupoMusicalContext _context;
        private IManequim _manequim;

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
            var manequins = new List<Manequim>
            {
                new Manequim
                {
                    Id = 1,
                    Tamanho = "P",
                    Descricao = "PEQUENO"
                },
                new Manequim
                {
                    Id = 2,
                    Tamanho = "M",
                    Descricao = "MÉDIO"
                },
                 new Manequim
                {
                    Id = 3,
                    Tamanho = "G",
                    Descricao = "GRANDE"
                }


            };
            _context.AddRange(manequins);
            _context.SaveChanges();

            _manequim = new ManequimService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            _manequim.Create(new Manequim
            {
                Id = 4,
                Tamanho = "EG",
                Descricao = "Extra Grande"
            });

            // Assert
            Assert.AreEqual(4, _manequim.GetAll().Count());
            var manequim = _manequim.Get(4);
            Assert.AreEqual(4, manequim.Id);
            Assert.AreEqual("EG", manequim.Tamanho);
            Assert.AreEqual("Extra Grande", manequim.Descricao);
        }

    }
}
