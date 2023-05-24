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

namespace GestaoGrupoMusicalWeb.Controllers.Test
{
    [TestClass()]
    public class GrupoMusicalControllerTests
    {


        private static GrupoMusicalController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var mokServer = new Mock<IGrupoMusical>();
            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new GrupoMusicalProfile())).CreateMapper();

            mokServer.Setup(server => server.GetAll()).Returns(GetTestGrupoMusicals());
            mokServer.Setup(server => server.Get(1)).Returns(GetTargetGrupoMusical());
            mokServer.Setup(service => service.Edit(It.IsAny<Grupomusical>()))
               .Verifiable();
            mokServer.Setup(service => service.Create(It.IsAny<Grupomusical>()))
                .Verifiable();

            _controller = new GrupoMusicalController(mokServer.Object, mapper);

        }

        [TestMethod()]
        public void IndexTest()
        {
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<GrupoMusicalViewModel>));

            List<GrupoMusicalViewModel> lista = (List<GrupoMusicalViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod()]
        public void DetailsTest()
        {
            var result = _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(GrupoMusicalViewModel));
            GrupoMusicalViewModel grupoMusicalView = (GrupoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, grupoMusicalView.Id);
            Assert.AreEqual("Sergipe Cultura", grupoMusicalView.Agencia);
            Assert.AreEqual("Centro", grupoMusicalView.Bairro);
            Assert.AreEqual("Banco do Brasil", grupoMusicalView.Banco);
            Assert.AreEqual("49520-111", grupoMusicalView.Cep);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.ChavePix);
            Assert.AreEqual("E-mail", grupoMusicalView.ChavePixtipo);
            Assert.AreEqual("Itabaina", grupoMusicalView.Cidade);
            Assert.AreEqual("10.875.926/0001-20", grupoMusicalView.Cnpj);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.Email);
            Assert.AreEqual("SE", grupoMusicalView.Estado);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Facebook);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Instagram);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Nome);
            Assert.AreEqual("78756-2", grupoMusicalView.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusicalView.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusicalView.RazaoSocial);
            Assert.AreEqual("Campo do Brito", grupoMusicalView.Rua);
            Assert.AreEqual("79999857845", grupoMusicalView.Telefone1);
            Assert.AreEqual("3433-7859", grupoMusicalView.Telefone2);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Youtube);
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
            var result = _controller.Create(GetNewGrupoMusical());

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
            var result = _controller.Create(GetNewGrupoMusical());

            // Assert
            Assert.AreEqual(1, _controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }


        private GrupoMusicalViewModel GetNewGrupoMusical()
        {
            return new GrupoMusicalViewModel
            {
                Id = 1,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "49520-111",
                ChavePix = "GrupoArraia@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Itabaina",
                Cnpj = "10.875.926/0001-20",
                Email = "GrupoArraia@gmail.com",
                Estado = "SE",
                Facebook = "Grupo Arraia",
                Instagram = "Grupo Arraia",
                Nome = "Grupo Arraia",
                NumeroContaBanco = "78756-2",
                Pais = "Brasil",
                RazaoSocial = "Participação Grupo musical ",
                Rua = "Campo do Brito",
                Telefone1 = "79999857845",
                Telefone2 = "3433-7859",
                Youtube = "Grupo Arraia",
            };

        }


        [TestMethod()]
        public void Edit_Get()
        {
            var result = _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(GrupoMusicalViewModel));
            GrupoMusicalViewModel grupoMusicalView = (GrupoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, grupoMusicalView.Id);
            Assert.AreEqual("Sergipe Cultura", grupoMusicalView.Agencia);
            Assert.AreEqual("Centro", grupoMusicalView.Bairro);
            Assert.AreEqual("Banco do Brasil", grupoMusicalView.Banco);
            Assert.AreEqual("49520-111", grupoMusicalView.Cep);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.ChavePix);
            Assert.AreEqual("E-mail", grupoMusicalView.ChavePixtipo);
            Assert.AreEqual("Itabaina", grupoMusicalView.Cidade);
            Assert.AreEqual("10.875.926/0001-20", grupoMusicalView.Cnpj);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.Email);
            Assert.AreEqual("SE", grupoMusicalView.Estado);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Facebook);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Instagram);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Nome);
            Assert.AreEqual("78756-2", grupoMusicalView.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusicalView.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusicalView.RazaoSocial);
            Assert.AreEqual("Campo do Brito", grupoMusicalView.Rua);
            Assert.AreEqual("79999857845", grupoMusicalView.Telefone1);
            Assert.AreEqual("3433-7859", grupoMusicalView.Telefone2);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Youtube);

        }

        [TestMethod()]
        public void Edit_Post()
        {
            // Act
            var result = _controller.Edit(GetTargetGrupoMusicalViewModel().Id, GetTargetGrupoMusicalViewModel());
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [TestMethod()]
        public void Delete_Get()
        {
            var result = _controller.Delete(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(GrupoMusicalViewModel));
            GrupoMusicalViewModel grupoMusicalView = (GrupoMusicalViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, grupoMusicalView.Id);
            Assert.AreEqual("Sergipe Cultura", grupoMusicalView.Agencia);
            Assert.AreEqual("Centro", grupoMusicalView.Bairro);
            Assert.AreEqual("Banco do Brasil", grupoMusicalView.Banco);
            Assert.AreEqual("49520-111", grupoMusicalView.Cep);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.ChavePix);
            Assert.AreEqual("E-mail", grupoMusicalView.ChavePixtipo);
            Assert.AreEqual("Itabaina", grupoMusicalView.Cidade);
            Assert.AreEqual("10.875.926/0001-20", grupoMusicalView.Cnpj);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusicalView.Email);
            Assert.AreEqual("SE", grupoMusicalView.Estado);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Facebook);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Instagram);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Nome);
            Assert.AreEqual("78756-2", grupoMusicalView.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusicalView.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusicalView.RazaoSocial);
            Assert.AreEqual("Campo do Brito", grupoMusicalView.Rua);
            Assert.AreEqual("79999857845", grupoMusicalView.Telefone1);
            Assert.AreEqual("3433-7859", grupoMusicalView.Telefone2);
            Assert.AreEqual("Grupo Arraia", grupoMusicalView.Youtube);

        }

        [TestMethod()]
        public void Delete_post()
        {
            //Act
            var result = _controller.Delete(GetTargetGrupoMusicalViewModel().Id, GetTargetGrupoMusicalViewModel());
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private Grupomusical GetTargetGrupoMusical()
        {
            return new Grupomusical
            {
                Id = 1,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "49520-111",
                ChavePix = "GrupoArraia@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Itabaina",
                Cnpj = "10.875.926/0001-20",
                Email = "GrupoArraia@gmail.com",
                Estado = "SE",
                Facebook = "Grupo Arraia",
                Instagram = "Grupo Arraia",
                Nome = "Grupo Arraia",
                NumeroContaBanco = "78756-2",
                Pais = "Brasil",
                RazaoSocial = "Participação Grupo musical ",
                Rua = "Campo do Brito",
                Telefone1 = "79999857845",
                Telefone2 = "3433-7859",
                Youtube = "Grupo Arraia",
            };
        }

        private IEnumerable<Grupomusical> GetTestGrupoMusicals()
        {
            return new List<Grupomusical>
            {
               new Grupomusical
               {
                Id = 1,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "49520-111",
                ChavePix = "GrupoArraia@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Itabaina",
                Cnpj = "10.875.926/0001-20",
                Email = "GrupoArraia@gmail.com",
                Estado = "SE",
                Facebook = "Grupo Arraia",
                Instagram = "Grupo Arraia",
                Nome = "Grupo Arraia",
                NumeroContaBanco = "78756-2",
                Pais = "Brasil",
                RazaoSocial =  "Participação Grupo musical ",
                Rua = "Campo do Brito",
                Telefone1 = "79999857845",
                Telefone2 = "3433-7859",
                Youtube = "Grupo Arraia",
               },
               new Grupomusical
               {
                Id = 2,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "49520-000",
                ChavePix = "grupobatala@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Aracaju",
                Cnpj = "55.875.926/0001-12",
                Email = "grupobatala@gmail.com",
                Estado = "SE",
                Facebook = "Grupo Batala",
                Instagram = "Grupobatala",
                Nome = "Grupo Batala",
                NumeroContaBanco = "78744-2",
                Pais = "Brasil",
                RazaoSocial =  "Participação Grupo musical ",
                Rua = "São Jão de Bispo",
                Telefone1 = "799854721",
                Telefone2 = "3433-1879",
                Youtube = "Grupo Batala",
               },
               new Grupomusical {Id = 3,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "42320-000",
                ChavePix = "ForrordoAgreste@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Campo do Brito",
                Cnpj = "55.875.926/0001-12",
                Email = "ForrordoAgrestea@gmail.com",
                Estado = "SE",
                Facebook = "Forror do Agreste",
                Instagram = "ForrordoAgreste",
                Nome = "Forror do Agreste",
                NumeroContaBanco = "69744-2",
                Pais = "Brasil",
                RazaoSocial =  "Participação Grupo musical ",
                Rua = "Maria Soares da Silveira",
                Telefone1 = "79999954731",
                Telefone2 = "3443-1879",
                Youtube = "Forror do Agreste",}
            };
        }
        private GrupoMusicalViewModel GetTargetGrupoMusicalViewModel()
        {
            return new GrupoMusicalViewModel
            {
                Id = 2,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Banco do Brasil",
                Cep = "49520-000",
                ChavePix = "grupobatala@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Aracaju",
                Cnpj = "55.875.926/0001-12",
                Email = "grupobatala@gmail.com",
                Estado = "SE",
                Facebook = "Grupo Batala",
                Instagram = "Grupobatala",
                Nome = "Grupo Batala",
                NumeroContaBanco = "78744-2",
                Pais = "Brasil",
                RazaoSocial = "Participação Grupo musical ",
                Rua = "São Jão de Bispo",
                Telefone1 = "799854721",
                Telefone2 = "3433-1879",
                Youtube = "Grupo Batala",

            };
        }

    }
}