using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestaoGrupoMusicalWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Service;
using Moq;
using AutoMapper;
using GestaoGrupoMusicalWeb.Mapper;
using GestaoGrupoMusicalWeb.Models;
using Core;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers.Tests
{
    [TestClass()]
    public class EventoControllerTests
    {
        private static EventoController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var mokServer = new Mock<IEventoService>();
            var mokServicePessoa = new Mock<IPessoaService>();
            var mokServiceGrupoMusical = new Mock<IGrupoMusical>();
            

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new EventoProfile())).CreateMapper();

            mokServer.Setup(server => server.GetAll()).Returns(GetTestEventos());
            mokServer.Setup(server => server.Get(1)).Returns(GetTargetEvento());
            mokServer.Setup(service => service.Edit(It.IsAny<Evento>()))
               .Verifiable();
            mokServer.Setup(service => service.Create(It.IsAny<Evento>()))
                .Verifiable();

            _controller = new EventoController(mokServer.Object, mapper, mokServiceGrupoMusical.Object, mokServicePessoa.Object);
        }

        [TestMethod()]
        public void IndexTest()
        {
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<EventoViewModel>));

            List<EventoViewModel> lista = (List<EventoViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod()]
        public void DetailsTest()
        {
            var result = _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EventoViewModel));
            EventoViewModel eventoView = (EventoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, eventoView.Id);
            Assert.AreEqual(new DateTime(2023, 7, 11, 8, 0, 0), eventoView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 11, 12, 0, 0), eventoView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", eventoView.Local);
            Assert.AreEqual("Ensaio de Percusão", eventoView.Repertorio);
            Assert.AreEqual(1, eventoView.IdColaboradorResponsavel);
            Assert.AreEqual(1, eventoView.IdRegente);
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
            var result = _controller.Create(GetNewEvento());

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
            var result = _controller.Create(GetNewEvento());

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
            var result = _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EventoViewModel));
            EventoViewModel eventoView = (EventoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, eventoView.Id);
            Assert.AreEqual(new DateTime(2023, 7, 11, 8, 0, 0), eventoView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 11, 12, 0, 0), eventoView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", eventoView.Local);
            Assert.AreEqual("Ensaio de Percusão", eventoView.Repertorio);
            Assert.AreEqual(1, eventoView.IdColaboradorResponsavel);
            Assert.AreEqual(1, eventoView.IdRegente);

        }

        [TestMethod()]
        public void Edit_Post()
        {

            // Act
            var result = _controller.Edit(GetTargetEventoViewModel().Id, GetTargetEventoViewModel());
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void Delete_Get()
        {
            var result = _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(EventoViewModel));
            EventoViewModel eventoView = (EventoViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, eventoView.Id);
            Assert.AreEqual(new DateTime(2023, 7, 11, 8, 0, 0), eventoView.DataHoraInicio);
            Assert.AreEqual(new DateTime(2023, 7, 11, 12, 0, 0), eventoView.DataHoraFim);
            Assert.AreEqual("Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se", eventoView.Local);
            Assert.AreEqual("Ensaio de Percusão", eventoView.Repertorio);
            Assert.AreEqual(1, eventoView.IdColaboradorResponsavel);
            Assert.AreEqual(1, eventoView.IdRegente);
        }

        [TestMethod()]
        public void Delete_post()
        {
            //Act
            var result = _controller.Delete(GetTargetEventoViewModel().Id, GetTargetEventoViewModel());
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private Evento GetTargetEvento()
        {
            return new Evento
            {
                Id = 1,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0),
                Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                Repertorio = "Ensaio de Percusão",
                IdColaboradorResponsavel = 1,
                IdRegente = 1
            };
        }
        private IEnumerable<Evento> GetTestEventos()
        {
            return new List<Evento>
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
        }
        private EventoViewModel GetTargetEventoViewModel()
        {
            return new EventoViewModel
            {
                Id = 2,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 6, 30, 14, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 30, 16, 0, 0),
                Local = "Ginásio Professor Lima, rua: Francisco bragança, 260, centro, Aracaju-se",
                Repertorio = "Recepcão de alunos novos",
                IdColaboradorResponsavel = 2,
                IdRegente = 1
            };
        }

        private EventoViewModel GetNewEvento()
        {
            return new EventoViewModel
            {
                Id = 1,
                IdGrupoMusical = 1,
                DataHoraInicio = new DateTime(2023, 7, 11, 8, 0, 0),
                DataHoraFim = new DateTime(2023, 7, 11, 12, 0, 0),
                Local = "Centro batalá, rua: percilio andrade, 130, centro, Aracaju-se",
                Repertorio = "Ensaio de Percusão",
                IdColaboradorResponsavel = 1,
                IdRegente = 1
            };


        }
    }
}
