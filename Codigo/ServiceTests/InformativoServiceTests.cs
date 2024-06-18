using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    [TestClass]
    public class InformativoServiceTests
    {
        private GrupoMusicalContext _context;
        private IInformativoService _informativoService;

        [TestInitialize]
        public void Initialize()
        {

            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            //_context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var materialestudos = new List<Informativo>
            {
                new Informativo
                {
                    IdGrupoMusical = 1,
                    IdPessoa = 1,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",                   
                    Data = new DateTime(2024, 6, 18, 8, 0, 0),
                    EntregarAssociadosAtivos = 1,                    
                },
                 new Informativo
                {
                    IdGrupoMusical = 2,
                    IdPessoa = 1,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 6, 19, 8, 0, 0),
                    EntregarAssociadosAtivos = 0,
                },
                  new Informativo
                {
                    IdGrupoMusical = 3,
                    IdPessoa = 1,
                    Mensagem = "Bom dia! lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 6, 19, 8, 0, 0),
                    EntregarAssociadosAtivos = 1,
                },

            };
            _context.AddRange(materialestudos);
            _context.SaveChanges();

            _informativoService = new InformativoService(_context);
        }
        [TestMethod]
        public void CreateTest()
        {
            // Act
            var newInformativo = new Informativo
            {
                IdGrupoMusical = 3,
                IdPessoa = 1,
                Mensagem = "Bom dia! lembrem-se o horário combinado.",
                Data = new DateTime(2024, 6, 19, 8, 0, 0),
                EntregarAssociadosAtivos = 1
            };

            _informativoService.Create(newInformativo);

            // Assert
            var informativo = _informativoService.Get(1, 3).Result;
            Assert.AreEqual(1, informativo.IdGrupoMusical);
            Assert.AreEqual(1, informativo.IdPessoa);
            Assert.AreEqual("Bom dia! lembrem-se o horário combinado.", informativo.Mensagem);            
            Assert.AreEqual(new DateTime(2024, 6, 19, 8, 0, 0), informativo.Data);
            Assert.AreEqual(1, informativo.EntregarAssociadosAtivos);
        }
        [TestMethod]
        public void DeleteTest()
        {
            var informativo = _context.Informativos.Find(1);
            Assert.IsNotNull(informativo, "Informativo não encontrado para deletar.");

            _context.Informativos.Remove(informativo);
            _context.SaveChanges();

            var deletedInformativo = _context.Informativos.Find(1);
            Assert.IsNull(deletedInformativo, "Informativo não foi deletado.");

        }

    }
}
