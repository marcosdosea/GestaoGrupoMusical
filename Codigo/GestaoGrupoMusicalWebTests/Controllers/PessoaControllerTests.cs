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
    public class PessoaControllerTests
    {


        private static PessoaController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var mokServer = new Mock<IPessoaService>();
            var mokserverGrupoMusical = new Mock<IGrupoMusical>();
            var mokserverPapelGrupo = new Mock<IPapelGrupo>();
            var mokserverManequim = new Mock<IManequim>();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddProfile(new PessoaProfile())).CreateMapper();

            mokServer.Setup(server => server.GetAll()).Returns(GetTestPessoas());
            mokServer.Setup(server => server.Get(1)).Returns(GetTargetPessoa());
            mokServer.Setup(service => service.Edit(It.IsAny<Pessoa>()))
               .Verifiable();
            mokServer.Setup(service => service.Create(It.IsAny<Pessoa>()))
                .Verifiable();

            // TODO: implementar o ".setup" para mokServerGrupoMusical
            // TODO: implementar o ".setup" para mokServerPapelGrupo
            // TODO: implementar o ".setup" para mokServerManequim

            _controller = new PessoaController(mokServer.Object, mapper, mokserverGrupoMusical.Object, mokserverPapelGrupo.Object, mokserverManequim.Object);

        }

        [TestMethod()]
        public void IndexTest()
        {
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(List<PessoaViewModel>));

            List<PessoaViewModel> lista = (List<PessoaViewModel>)viewResult.ViewData.Model;
            Assert.AreEqual(3, lista.Count);
        }

        [TestMethod()]
        public void DetailsTest()
        {
            var result = _controller.Details(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PessoaViewModel));
            PessoaViewModel pessoaView = (PessoaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, pessoaView.Id);
            Assert.AreEqual("007.587.624-02", pessoaView.Cpf);
            Assert.AreEqual("José santos", pessoaView.Nome);
            Assert.AreEqual("M", pessoaView.Sexo);
            Assert.AreEqual("49520-111", pessoaView.Cep);
            Assert.AreEqual("Rua 10", pessoaView.Rua);
            Assert.AreEqual("Centro", pessoaView.Bairro);
            Assert.AreEqual("Itabaina", pessoaView.Cidade);
            Assert.AreEqual("SE", pessoaView.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1992"), pessoaView.DataNascimento);
            Assert.AreEqual("79998567896", pessoaView.Telefone1);
            Assert.AreEqual("79998653284", pessoaView.Telefone2);
            Assert.AreEqual("josests@gmail.com", pessoaView.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2020"), pessoaView.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2022"), pessoaView.DataSaida);
            Assert.AreEqual("Não me acostumei com a cultura", pessoaView.MotivoSaida);
            Assert.AreEqual(1, pessoaView.Ativo);
            Assert.AreEqual(1, pessoaView.IdGrupoMusical);
            Assert.AreEqual(1, pessoaView.IdPapelGrupo);
            Assert.AreEqual(1, pessoaView.IdManequim);
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
            var result = _controller.Create(GetNewPessoa());

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
            var result = _controller.Create(GetNewPessoa());

            // Assert
            Assert.AreEqual(1, _controller.ModelState.ErrorCount);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }


        private PessoaViewModel GetNewPessoa()
        {
            return new PessoaViewModel
            {
                Id = 1,
                Cpf = "007.587.624-02",
                Nome = "José santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-02-1992"),
                Telefone1 = "79998567896",
                Telefone2 = "79998653284",
                Email = "josests@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2020"),
                DataSaida = DateTime.Parse("05-06-2022"),
                MotivoSaida = "Não me acostumei com a cultura",
                Ativo = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 1,
                IdManequim = 1
            };

        }


        [TestMethod()]
        public void Edit_Get()
        {
            var result = _controller.Edit(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PessoaViewModel));
            PessoaViewModel pessoaView = (PessoaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, pessoaView.Id);
            Assert.AreEqual("007.587.624-02", pessoaView.Cpf);
            Assert.AreEqual("José santos", pessoaView.Nome);
            Assert.AreEqual("M", pessoaView.Sexo);
            Assert.AreEqual("49520-111", pessoaView.Cep);
            Assert.AreEqual("Rua 10", pessoaView.Rua);
            Assert.AreEqual("Centro", pessoaView.Bairro);
            Assert.AreEqual("Itabaina", pessoaView.Cidade);
            Assert.AreEqual("SE", pessoaView.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1992"), pessoaView.DataNascimento);
            Assert.AreEqual("79998567896", pessoaView.Telefone1);
            Assert.AreEqual("79998653284", pessoaView.Telefone2);
            Assert.AreEqual("josests@gmail.com", pessoaView.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2020"), pessoaView.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2022"), pessoaView.DataSaida);
            Assert.AreEqual("Não me acostumei com a cultura", pessoaView.MotivoSaida);
            Assert.AreEqual(1, pessoaView.Ativo);
            Assert.AreEqual(1, pessoaView.IdGrupoMusical);
            Assert.AreEqual(1, pessoaView.IdPapelGrupo);
            Assert.AreEqual(1, pessoaView.IdManequim);

        }

        [TestMethod()]
        public void Edit_Post()
        {
            // Act
            var result = _controller.Edit(GetTargetPessoaViewModel().Id, GetTargetPessoaViewModel());
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
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(PessoaViewModel));
            PessoaViewModel pessoaView = (PessoaViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(1, pessoaView.Id);
            Assert.AreEqual("007.587.624-02", pessoaView.Cpf);
            Assert.AreEqual("José santos", pessoaView.Nome);
            Assert.AreEqual("M", pessoaView.Sexo);
            Assert.AreEqual("49520-111", pessoaView.Cep);
            Assert.AreEqual("Rua 10", pessoaView.Rua);
            Assert.AreEqual("Centro", pessoaView.Bairro);
            Assert.AreEqual("Itabaina", pessoaView.Cidade);
            Assert.AreEqual("SE", pessoaView.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1992"), pessoaView.DataNascimento);
            Assert.AreEqual("79998567896", pessoaView.Telefone1);
            Assert.AreEqual("79998653284", pessoaView.Telefone2);
            Assert.AreEqual("josests@gmail.com", pessoaView.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2020"), pessoaView.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2022"), pessoaView.DataSaida);
            Assert.AreEqual("Não me acostumei com a cultura", pessoaView.MotivoSaida);
            Assert.AreEqual(1, pessoaView.Ativo);
            Assert.AreEqual(1, pessoaView.IdGrupoMusical);
            Assert.AreEqual(1, pessoaView.IdPapelGrupo);
            Assert.AreEqual(1, pessoaView.IdManequim);
        }

        [TestMethod()]
        public void Delete_post()
        {
            //Act
            var result = _controller.Delete(GetTargetPessoa().Id, GetTargetPessoaViewModel());
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            Assert.IsNull(redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        private Pessoa GetTargetPessoa()
        {
            return new Pessoa
            {
                Id = 1,
                Cpf = "007.587.624-02",
                Nome = "José santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-02-1992"),
                Telefone1 = "79998567896",
                Telefone2 = "79998653284",
                Email = "josests@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2020"),
                DataSaida = DateTime.Parse("05-06-2022"),
                MotivoSaida = "Não me acostumei com a cultura",
                Ativo = 1,
                IsentoPagamento = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 1,
                IdManequim = 1
            };
        }

        private IEnumerable<Pessoa> GetTestPessoas()
        {
            return new List<Pessoa>
            {
               new Pessoa
               {
                Id = 1,
                Cpf = "007.567.555-02",
                Nome = "Fernando santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-06-1995"),
                Telefone1 = "79999467896",
                Telefone2 = "79998621284",
                Email = "fernandosts@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2020"),
                DataSaida = DateTime.Parse("05-06-2022"),
                MotivoSaida = "Não me entrosei bem com o pessoal",
                Ativo = 1,
                IsentoPagamento = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 2,
                IdManequim = 3
               },
               new Pessoa
               {               
                Id = 2,
                Cpf = "007.457.624-02",
                Nome = "Matheus santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-02-1985"),
                Telefone1 = "79956567896",
                Telefone2 = "79998653654",
                Email = "matheussts@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2020"),
                DataSaida = DateTime.Parse("05-06-2022"),
                MotivoSaida = "Não me acostumei com a cultura",
                Ativo = 1,
                IsentoPagamento = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 1,
                IdManequim = 1
               },
               new Pessoa {
                   Id = 3,
                   Cpf = "007.217.424-02",
                   Nome = "Douglas santos",
                   Sexo = "M",
                   Cep = "49520-111",
                   Rua = "Rua 10",
                   Bairro = "Centro",
                   Cidade = "Itabaina",
                   Estado = "SE",
                   DataNascimento = DateTime.Parse("01-02-1998"),
                   Telefone1 = "79998567896",
                   Telefone2 = "79998653284",
                   Email = "Douglassts@gmail.com",
                   DataEntrada = DateTime.Parse("10-01-2019"),
                   DataSaida = DateTime.Parse("05-06-2021"),
                   MotivoSaida = "Não me acostumei com a cultura",
                   Ativo = 1,
                   IsentoPagamento = 1,
                   IdGrupoMusical = 1,
                   IdPapelGrupo = 1,
                   IdManequim = 1
               }
            };
        }
        private PessoaViewModel GetTargetPessoaViewModel()
        {
            return new PessoaViewModel
            {
                Id = 1,
                Cpf = "007.587.624-02",
                Nome = "José santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-02-1992"),
                Telefone1 = "79998567896",
                Telefone2 = "79998653284",
                Email = "josests@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2020"),
                DataSaida = DateTime.Parse("05-06-2022"),
                MotivoSaida = "Não me acostumei com a cultura",
                Ativo = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 1,
                IdManequim = 1,

            };
        }

    }
}