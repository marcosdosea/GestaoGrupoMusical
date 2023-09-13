using Microsoft.VisualStudio.TestTools.UnitTesting;
using GestaoGrupoMusicalWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using GestaoGrupoMusicalWeb.Models;
using Core.Service;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GestaoGrupoMusicalWeb.Mapper;

namespace GestaoGrupoMusicalWeb.Controllers.Tests
{
    [TestClass()]
    public class ColaboradorControllerTests
    {
        /*
        private static ColaboradorController _controller;

        [TestInitialize]
        public void Initialize()
        {
            var MokServer = new Mock<IPessoaService>();
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile(new PessoaProfile())).CreateMapper();


            MokServer.Setup(server => server.GetAll()).Returns(GetTestPessoas());
            MokServer.Setup(server => server.Get(1)).Returns(GetTargetPessoa());
            MokServer.Setup(service => service.Edit(It.IsAny<Pessoa>()))
               .Verifiable();
            MokServer.Setup(service => service.Create(It.IsAny<Pessoa>()))
                .Verifiable();
            _controller = new ColaboradorController(MokServer.Object, mapper);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            var result = _controller.Create(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod()]
        public void CreateTest_Post_Valid()
        {
            // Act
            var result = _controller.Create(1);

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
        public void CreateTest_Post_Invalid()
        {
            // Act
            var result = _controller.Create(GetTargetPessoaViewModel().Id, GetTargetPessoaViewModel());
            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
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
        }*/

    

    }
}