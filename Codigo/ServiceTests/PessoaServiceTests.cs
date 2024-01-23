using Core.Service;
using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass()]
    public class PessoaServiceTests
    {

        private GrupoMusicalContext _context;
        private IPessoaService _pessoaService;
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
            var pessoas = new List<Pessoa>
                {
                    new Pessoa {
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
                    },
                    new Pessoa {
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

            _context.AddRange(pessoas);
            _context.SaveChanges();
            // TODO: criar os objetos UserManager, UserStore e RoleManager para o construtor abaixo (o de PessoaService)
            _pessoaService = new PessoaService(_context, null, null, null); 
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            _pessoaService.Create(new Pessoa()
            {
                Id = 4,
                Cpf = "007.789.024-02",
                Nome = "Jorge santos",
                Sexo = "M",
                Cep = "49520-111",
                Rua = "Rua 10",
                Bairro = "Centro",
                Cidade = "Itabaina",
                Estado = "SE",
                DataNascimento = DateTime.Parse("01-02-1998"),
                Telefone1 = "79996565896",
                Telefone2 = "79998453284",
                Email = "jorgests@gmail.com",
                DataEntrada = DateTime.Parse("10-01-2019"),
                DataSaida = DateTime.Parse("05-06-2021"),
                MotivoSaida = "Discuti com alguns membros",
                Ativo = 1,
                IsentoPagamento = 1,
                IdGrupoMusical = 1,
                IdPapelGrupo = 1,
                IdManequim = 1
            });
            // Assert
            Assert.AreEqual(4, _pessoaService.GetAll().Count());
            var pessoa = _pessoaService.Get(4);
            Assert.AreEqual(4, pessoa.Id);
            Assert.AreEqual("00778902402", pessoa.Cpf);
            Assert.AreEqual("Jorge santos", pessoa.Nome);
            Assert.AreEqual("M", pessoa.Sexo);
            Assert.AreEqual("49520111", pessoa.Cep);
            Assert.AreEqual("Rua 10", pessoa.Rua);
            Assert.AreEqual("Centro", pessoa.Bairro);
            Assert.AreEqual("Itabaina", pessoa.Cidade);
            Assert.AreEqual("SE", pessoa.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1998"), pessoa.DataNascimento);
            Assert.AreEqual("79996565896", pessoa.Telefone1);
            Assert.AreEqual("79998453284", pessoa.Telefone2);
            Assert.AreEqual("jorgests@gmail.com", pessoa.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2019"), pessoa.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2021"), pessoa.DataSaida);
            Assert.AreEqual("Discuti com alguns membros", pessoa.MotivoSaida);
            Assert.AreEqual(1, pessoa.Ativo);
            Assert.AreEqual(1, pessoa.IdGrupoMusical);
            Assert.AreEqual(1, pessoa.IdPapelGrupo);
            Assert.AreEqual(1, pessoa.IdManequim);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            _pessoaService.Delete(2);
            // Assert
            Assert.AreEqual(2, _pessoaService.GetAll().Count());
            var pessoa = _pessoaService.Get(2);
            Assert.AreEqual(null, pessoa);
        }

        [TestMethod()]
        public void EditTest()
        {
            var pessoa = _pessoaService.Get(3);
            pessoa.Id = 4;
            pessoa.Cpf = "007.789.024-02";
            pessoa.Nome = "Jorge santos";
            pessoa.Sexo = "M";
            pessoa.Cep = "49520-111";
            pessoa.Rua = "Rua 10";
            pessoa.Bairro = "Centro";
            pessoa.Cidade = "Itabaina";
            pessoa.Estado = "SE";
            pessoa.DataNascimento = DateTime.Parse("01-02-1998");
            pessoa.Telefone1 = "79996565896";
            pessoa.Telefone2 = "79998453284";
            pessoa.Email = "jorgests.silva@gmail.com";
            pessoa.DataEntrada = DateTime.Parse("10-01-2019");
            pessoa.DataSaida = DateTime.Parse("05-06-2021");
            pessoa.MotivoSaida = "Discuti com alguns membros";
            pessoa.Ativo = 1;
            pessoa.IdGrupoMusical = 1;
            pessoa.IdPapelGrupo = 2;
            pessoa.IdManequim = 3;

            //Assert

            Assert.AreEqual(4, pessoa.Id);
            Assert.AreEqual("007.789.024-02", pessoa.Cpf);
            Assert.AreEqual("Jorge santos", pessoa.Nome);
            Assert.AreEqual("M", pessoa.Sexo);
            Assert.AreEqual("49520-111", pessoa.Cep);
            Assert.AreEqual("Rua 10", pessoa.Rua);
            Assert.AreEqual("Centro", pessoa.Bairro);
            Assert.AreEqual("Itabaina", pessoa.Cidade);
            Assert.AreEqual("SE", pessoa.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1998"), pessoa.DataNascimento);
            Assert.AreEqual("79996565896", pessoa.Telefone1);
            Assert.AreEqual("79998453284", pessoa.Telefone2);
            Assert.AreEqual("jorgests.silva@gmail.com", pessoa.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2019"), pessoa.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2021"), pessoa.DataSaida);
            Assert.AreEqual("Discuti com alguns membros", pessoa.MotivoSaida);
            Assert.AreEqual(1, pessoa.Ativo);
            Assert.AreEqual(1, pessoa.IdGrupoMusical);
            Assert.AreEqual(2, pessoa.IdPapelGrupo);
            Assert.AreEqual(3, pessoa.IdManequim);
        }

        [TestMethod()]
        public void GetTest()
        {
            var pessoa = _pessoaService.Get(1);
            Assert.AreEqual(1, pessoa.Id);
            Assert.AreEqual("007.587.624-02", pessoa.Cpf);
            Assert.AreEqual("José santos", pessoa.Nome);
            Assert.AreEqual("M", pessoa.Sexo);
            Assert.AreEqual("49520-111", pessoa.Cep);
            Assert.AreEqual("Rua 10", pessoa.Rua);
            Assert.AreEqual("Centro", pessoa.Bairro);
            Assert.AreEqual("Itabaina", pessoa.Cidade);
            Assert.AreEqual("SE", pessoa.Estado);
            Assert.AreEqual(DateTime.Parse("01-02-1992"), pessoa.DataNascimento);
            Assert.AreEqual("79998567896", pessoa.Telefone1);
            Assert.AreEqual("79998653284", pessoa.Telefone2);
            Assert.AreEqual("josests@gmail.com", pessoa.Email);
            Assert.AreEqual(DateTime.Parse("10-01-2020"), pessoa.DataEntrada);
            Assert.AreEqual(DateTime.Parse("05-06-2022"), pessoa.DataSaida);
            Assert.AreEqual("Não me acostumei com a cultura", pessoa.MotivoSaida);
            Assert.AreEqual(1, pessoa.Ativo);
            Assert.AreEqual(1, pessoa.IdGrupoMusical);
            Assert.AreEqual(1, pessoa.IdPapelGrupo);
            Assert.AreEqual(1, pessoa.IdManequim);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaPessoa = _pessoaService.GetAll();
            // Assert
            Assert.IsInstanceOfType(listaPessoa, typeof(IEnumerable<Pessoa>));
            Assert.IsNotNull(listaPessoa);
            Assert.AreEqual(3, listaPessoa.Count());
            Assert.AreEqual(1, listaPessoa.First().Id);
        }


    }

}