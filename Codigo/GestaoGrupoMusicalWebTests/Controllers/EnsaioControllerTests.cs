using AutoMapper;
using Core.DTO;
using Core.Service;
using Core;
using GestaoGrupoMusicalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestaoGrupoMusicalWeb.Mapper;

namespace GestaoGrupoMusicalWeb.Controllers.Tests
{
    [TestClass()]
    public class EnsaioControllerTests
    {
        /*
        private static EnsaioController _controller;


        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var mokServer = new Mock<IEnsaioService>();
            var mokServerPessoa = new Mock<IPessoaService>();
            var mokServerMovimentacao = new Mock<IGrupoMusicalService>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new EnsaioProfile())).CreateMapper();

            mokServer.Setup(server => server.GetAllDTO().Result).Returns(GetTestEnsaioDTO());
            mokServer.Setup(server => server.GetAll().Result).Returns(GetTestEnsaios());
            mokServer.Setup(server => server.Get(1).Result).Returns(GetTargetEnsaio());
            mokServer.Setup(service => service.Edit(It.IsAny<Ensaio>()).Result).Returns(true)
               .Verifiable();
            mokServer.Setup(service => service.Create(It.IsAny<Ensaio>()).Result).Returns(true)
                .Verifiable();

            // TODO: implementar o ".setup" para mokServerPessoa
            // TODO: implementar o ".setup" para mokServerMovimentacao
            _controller = new EnsaioController(mapper, mokServer.Object, mokServerPessoa.Object, mokServerMovimentacao.Object);
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<EnsaioViewModel>));

            List<EnsaioViewModel> lista = (List<EnsaioViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count());
        }

        [TestMethod()]
        public void DetailsTest()
        {
            //Act
            var result = _controller.Details(1).GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EnsaioViewModel));
            EnsaioViewModel ensaioView = (EnsaioViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, ensaioView.Id);
            Assert.AreEqual(1, ensaioView.IdGrupoMusical);
            Assert.AreEqual(new DateTime(2023, 8, 11, 8, 0, 0), ensaioView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 8, 11, 12, 0, 0), ensaioView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", ensaioView.Local);
            Assert.AreEqual("Ensaio de Percusão", ensaioView.Repertorio);
            Assert.AreEqual(1, ensaioView.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaioView.IdRegente);
            Assert.AreEqual(true, ensaioView.PresencaObrigatoria);
            Assert.AreEqual(Tipo.Extra, ensaioView.Tipo);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var result = _controller.Create();
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = _controller.Create(GetNewEnsaio()).GetAwaiter().GetResult();

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
            _controller.ModelState.AddModelError("Data hora inicio", "Campo requerido");

            // Act
            var result = _controller.Create(GetNewEnsaio()).Result;

            // Assert
            Assert.AreEqual(1, _controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            var model = (viewResult.ViewData.Model) as EnsaioViewModel;
            Assert.IsFalse(_controller.ModelState.IsValid);

        }

        [TestMethod()]
        public void Edit_Get()
        {
            //Arrange
            var result = _controller.Edit(1).Result;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EnsaioViewModel));
            EnsaioViewModel ensaioView = (EnsaioViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, ensaioView.Id);
            Assert.AreEqual(1, ensaioView.IdGrupoMusical);
            Assert.AreEqual(new DateTime(2023, 8, 11, 8, 0, 0), ensaioView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 8, 11, 12, 0, 0), ensaioView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", ensaioView.Local);
            Assert.AreEqual("Ensaio de Percusão", ensaioView.Repertorio);
            Assert.AreEqual(1, ensaioView.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaioView.IdRegente);
            Assert.AreEqual(true, ensaioView.PresencaObrigatoria);
            Assert.AreEqual(Tipo.Extra, ensaioView.Tipo);
        }

        [TestMethod()]
        public void Edit_Post()
        {
            // Act
            var result = _controller.Edit(GetTargetEnsaioViewModel()).Result;
            
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EnsaioViewModel));
            EnsaioViewModel ensaioView = (EnsaioViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, ensaioView.Id);
            Assert.AreEqual(1, ensaioView.IdGrupoMusical);
            Assert.AreEqual(new DateTime(2023, 8, 11, 8, 0, 0), ensaioView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 8, 11, 12, 0, 0), ensaioView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", ensaioView.Local);
            Assert.AreEqual("Ensaio de Percusão", ensaioView.Repertorio);
            Assert.AreEqual(1, ensaioView.IdColaboradorResponsavel);
            Assert.AreEqual(1, ensaioView.IdRegente);
            Assert.AreEqual(true, ensaioView.PresencaObrigatoria);
            Assert.AreEqual(Tipo.Extra, ensaioView.Tipo);
        }


        [TestMethod()]
        public void Delete_post()
        {
            //Act
            var result = _controller.Delete(GetTargetEnsaioViewModel().Id).Result;
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            var model = (viewResult.ViewData.Model) as EnsaioViewModel;
            Assert.IsNull(model);
        }

        private Ensaio GetTargetEnsaio()
        {
            return new Ensaio
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
            };
        }

        private EnsaioViewModel GetTargetEnsaioViewModel()
        {
            return new EnsaioViewModel
            {
                Id = 2,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0),
                Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                Repertorio = "Quando vi a terra ardendo",
                IdColaboradorResponsavel = 1,
                IdRegente = 1,
                PresencaObrigatoria = true,
                Tipo = Tipo.Fixo,
          
            };
        }

        private IEnumerable<Ensaio> GetTestEnsaios()
        {
            return new List<Ensaio>
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
        }

        private EnsaioViewModel GetNewEnsaio()
        {
            return new EnsaioViewModel
            {
                Id = 1,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0),
                Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                Repertorio = "Ensaio de Percusão",
                IdColaboradorResponsavel = 1,
                IdRegente = 1,
                PresencaObrigatoria = true,
                Tipo = Tipo.Extra,
            };
        }

        private IEnumerable<EnsaioDTO> GetTestEnsaioDTO()
        {
            return new List<EnsaioDTO>
            {
                new EnsaioDTO
                {
                    Local = "Cento batala em aracaju",
                    DataHoraInicio = DateTime.Now
                },
                new EnsaioDTO
                {
                    Local = "Cento batala em itabaiana",
                    DataHoraInicio = DateTime.Now
                }
            };
        }*/
    }
}
