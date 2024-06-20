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
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var materialestudos = new List<Informativo>
            {
                new Informativo
                {
                    IdGrupoMusical = 1,
                    IdPessoa = 3,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",                   
                    Data = new DateTime(2024, 6, 18, 8, 0, 0),
                    EntregarAssociadosAtivos = 1,                    
                },
                 new Informativo
                {
                    IdGrupoMusical = 1,
                    IdPessoa = 2,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 06, 19, 14, 0, 0),
                    EntregarAssociadosAtivos = 1,
                },
                  new Informativo
                {
                    IdGrupoMusical = 1,
                    IdPessoa = 4,
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
                IdGrupoMusical = 1,
                IdPessoa = 3,
                Mensagem = "Bom dia! lembrem-se o horário combinado.",
                Data = new DateTime(2024, 06, 30, 14, 0, 0),
                EntregarAssociadosAtivos = 1
            };

            _informativoService.Create(newInformativo);

            // Assert
            var informativo = _informativoService.Get(1, 3).Result;
            Assert.AreEqual(1, informativo.IdGrupoMusical);
            Assert.AreEqual(3, informativo.IdPessoa);
            Assert.AreEqual("Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.", informativo.Mensagem);            
            Assert.AreEqual(new DateTime(2024, 6, 18, 8, 0, 0), informativo.Data);
            Assert.AreEqual(1, informativo.EntregarAssociadosAtivos);

        }
        
        [TestMethod]
        public void DeleteTest()
        {
            var informativo = _context.Informativos.Find(1, 3);
            Assert.IsNotNull(informativo, "Informativo não encontrado para deletar.");

            _context.Informativos.Remove(informativo);
            _context.SaveChanges();

            var deletedInformativo = _context.Informativos.Find(1,3);
            Assert.IsNull(deletedInformativo, "Informativo não foi deletado.");

        }
        
        [TestMethod]
        public void EditTest()
        {
            // Arrange
            var informativoServiceTeste = _context.Informativos.Find(1, 3);
            informativoServiceTeste.IdGrupoMusical = 1;
            informativoServiceTeste.IdPessoa = 2;
            informativoServiceTeste.Mensagem = "Bom dia! lembrem-se o horário combinado.";
            informativoServiceTeste.Data = (new DateTime(2024, 06, 15, 10, 0, 0));
            informativoServiceTeste.EntregarAssociadosAtivos = 1;
        
            Assert.AreEqual(1, informativoServiceTeste.IdGrupoMusical);
            Assert.AreEqual(2, informativoServiceTeste.IdPessoa);
            Assert.AreEqual("Bom dia! lembrem-se o horário combinado.", informativoServiceTeste.Mensagem);            
            Assert.AreEqual(new DateTime(2024, 6, 15, 10, 0, 0), informativoServiceTeste.Data);
            Assert.AreEqual(1, informativoServiceTeste.EntregarAssociadosAtivos);
            
        }
        [TestMethod]
        public void GetTest()
        {
            var informativoService = _informativoService.Get(1, 2).Result;

            Assert.IsNotNull(informativoService, "Retornou Nulo.");

            Assert.AreEqual(1, informativoService.IdGrupoMusical);
            Assert.AreEqual(2, informativoService.IdPessoa);
            Assert.AreEqual("Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.", informativoService.Mensagem);
            Assert.AreEqual(new DateTime(2024, 06, 19, 14, 0, 0), informativoService.Data);
            Assert.AreEqual(1, informativoService.EntregarAssociadosAtivos);            
        }
        
        [TestMethod]
        public void GetAllTest()
        {
            var informativoService = _informativoService.GetAll().Result;
            Assert.AreEqual(3, informativoService.Count());
        }
        
        [TestMethod]
        public void GetAll()
        {
            var informativoService = _informativoService.GetAllInformativoServiceIdGrupo(1,2).Result;
            Assert.IsNotNull(informativoService);
            Assert.AreEqual(1, informativoService.Count());
        }
    }
}
