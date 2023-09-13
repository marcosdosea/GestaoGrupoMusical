using AutoMapper;
using Core;
using Core.DTO;
using Core.Service;
using GestaoGrupoMusicalWeb.Controllers;
using GestaoGrupoMusicalWeb.Mapper;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GestaoGrupoMusicalWeb.Controllers.Tests
{
    [TestClass()]
    public class InstrumentoMusicalControllerTests
    {
        /*
        private static InstrumentoMusicalController _controller;


        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var mokServer = new Mock<IInstrumentoMusicalService>();
            var mokServerPessoa = new Mock<IPessoaService>();
            var mokServerMovimentacao = new Mock<IMovimentacaoInstrumentoService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new InstrumentoMusicalProfile())).CreateMapper();

            mokServer.Setup(server => server.GetAllDTO().Result).Returns(GetTestInstrumentosMusicaisDTO());
            mokServer.Setup(server => server.GetAll().Result).Returns(GetTestInstrumentosMusicais());
            mokServer.Setup(server => server.Get(1).Result).Returns(GetTargetInstrumentoMusical());
            mokServer.Setup(service => service.Edit(It.IsAny<Instrumentomusical>()))
               .Verifiable();
            mokServer.Setup(service => service.Create(It.IsAny<Instrumentomusical>()))
                .Verifiable();

            // TODO: implementar o ".setup" para mokServerPessoa
            // TODO: implementar o ".setup" para mokServerMovimentacao
            _controller = new InstrumentoMusicalController(mokServer.Object, mokServerPessoa.Object, mokServerMovimentacao.Object, mapper);
        }

        [TestMethod()]
        public void IndexTest()
        {
            //Act
            var result = _controller.Index().GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<InstrumentoMusicalDTO>));

            List<InstrumentoMusicalDTO> lista = (List<InstrumentoMusicalDTO>)viewResult.ViewData.Model;
            Assert.AreEqual(2, lista.Count());
        }

        [TestMethod()]
        public void DetailsTest()
        {
            //Act
            var result = _controller.Details(1).GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(InstrumentoMusicalViewModel));
            InstrumentoMusicalViewModel instrumentoMusicalView = (InstrumentoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(5, instrumentoMusicalView.Id);
            Assert.AreEqual("5", instrumentoMusicalView.Patrimonio);
            Assert.AreEqual(DateTime.Parse("08/02/2023"), instrumentoMusicalView.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", instrumentoMusicalView.Status);
            Assert.AreEqual(0, instrumentoMusicalView.IdGrupoMusical);
            Assert.AreEqual(6, instrumentoMusicalView.IdTipoInstrumento);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var result = _controller.Create().Result;
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = _controller.Create(GetNewInstrumentoMusical()).GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void CreateTest_Post_Invalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Nome", "Campo requerido");

            // Act
            var result = _controller.Create(GetNewInstrumentoMusical()).Result;

            // Assert
            Assert.AreEqual(1, _controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void Edit_Get()
        {
            //Arrange
            var result = _controller.Edit(1).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(InstrumentoMusicalViewModel));
            InstrumentoMusicalViewModel instrumentoMusicalView = (InstrumentoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(5, instrumentoMusicalView.Id);
            Assert.AreEqual("5", instrumentoMusicalView.Patrimonio);
            Assert.AreEqual(DateTime.Parse("08/02/2023"), instrumentoMusicalView.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", instrumentoMusicalView.Status);
            Assert.AreEqual(6, instrumentoMusicalView.IdTipoInstrumento);
            Assert.AreEqual(0, instrumentoMusicalView.IdGrupoMusical);
        }

        [TestMethod()]
        public void Edit_Post()
        {
            // Act
            var result = _controller.Edit(GetTargetInstrumentoMusicalViewModel().Id, GetTargetInstrumentoMusicalViewModel()).Result;
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void Delete_Get()
        {
            var result = _controller.Delete(1).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(InstrumentoMusicalViewModel));
            InstrumentoMusicalViewModel instrumentoMusicalView = (InstrumentoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(5, instrumentoMusicalView.Id);
            Assert.AreEqual("5", instrumentoMusicalView.Patrimonio);
            Assert.AreEqual(DateTime.Parse("08/02/2023"), instrumentoMusicalView.DataAquisicao);
            Assert.AreEqual("DISPONIVEL", instrumentoMusicalView.Status);
            Assert.AreEqual(6, instrumentoMusicalView.IdTipoInstrumento);
            Assert.AreEqual(0, instrumentoMusicalView.IdGrupoMusical);
        }
    

        [TestMethod()]
        public void Delete_post()
        {
            //Act
            var result = _controller.Delete(GetTargetInstrumentoMusical().Id, GetTargetInstrumentoMusicalViewModel()).Result;
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private Instrumentomusical GetTargetInstrumentoMusical()
        {
            return new Instrumentomusical
            {
                Id = 5,
                Patrimonio = "5",
                DataAquisicao = DateTime.Parse("08/02/2023"),
                Status = "DISPONIVEL",
                IdTipoInstrumento = 6,
                IdGrupoMusical = 0
            };
        }

        private InstrumentoMusicalViewModel GetTargetInstrumentoMusicalViewModel()
        {
            return new InstrumentoMusicalViewModel
            {
                Id = 5,
                Patrimonio = "8",
                DataAquisicao = DateTime.Parse("20/02/2023"),
                Status = "DANIFICADO",
                IdTipoInstrumento = 6,
                IdGrupoMusical = 0
            };
        }

        private IEnumerable<Instrumentomusical> GetTestInstrumentosMusicais()
        {
            return new List<Instrumentomusical>
            {
                new Instrumentomusical
                {
                    Id = 6,
                    Patrimonio = "6",
                    DataAquisicao = DateTime.Parse("07/02/2021"),
                    Status = "DISPONIVEL",
                    IdTipoInstrumento = 0,
                    IdGrupoMusical = 0
                },
                new Instrumentomusical
                {
                    Id = 11,
                    Patrimonio = "200",
                    DataAquisicao = DateTime.Parse("05/03/2023"),
                    Status = "EMPRESTADO",
                    IdTipoInstrumento = 20,
                    IdGrupoMusical = 10
                },
                new Instrumentomusical
                {
                    Id = 12,
                    Patrimonio = "12",
                    DataAquisicao = DateTime.Parse("05/03/2023"),
                    Status = "DISPONIVEL",
                    IdTipoInstrumento = 12,
                    IdGrupoMusical = 3
                }
            };
        }

        private InstrumentoMusicalViewModel GetNewInstrumentoMusical()
        {
            return new InstrumentoMusicalViewModel
            {
                Id = 1,
                Patrimonio = "1",
                DataAquisicao = DateTime.Parse("24/02/2013"),
                Status = "DISPONIVEL",
                IdTipoInstrumento = 0,
                IdGrupoMusical = 0
            };
        }

        private IEnumerable<InstrumentoMusicalDTO> GetTestInstrumentosMusicaisDTO()
        {
            return new List<InstrumentoMusicalDTO>
            {
                new InstrumentoMusicalDTO
                {
                    Id = 6,
                    Patrimonio = "6",
                    NomeInstrumento = "Tambor",
                    Status = "DISPONIVEL",
                    NomeAssociado = "João Arlindo Santana"
                },
                new InstrumentoMusicalDTO
                {
                    Id = 7,
                    Patrimonio = "7",
                    NomeInstrumento = "Flauta",
                    Status = "EMPRESTADO",
                    NomeAssociado = "Maria Joana da Silva"
                }
            };
        }*/

    }
}
