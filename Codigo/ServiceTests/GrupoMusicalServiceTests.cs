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
using Core.DTO;

namespace Service.Tests
{
    [TestClass()]
    public class GrupoMusicalServiceTests
    {

        private GrupoMusicalContext _context;
        private IGrupoMusicalService _grupoMusical;
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
            var grupoMusicais = new List<Grupomusical>
                {
                    new Grupomusical {Id = 1,
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
                Youtube = "Grupo Arraia", },
                    new Grupomusical {Id = 2,
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
                Youtube = "Grupo Batala",},
                    new Grupomusical { Id = 3,
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
                Youtube = "Forror do Agreste",},
                };

            _context.AddRange(grupoMusicais);
            _context.SaveChanges();

            _grupoMusical = new GrupoMusicalService(_context);
        }

        [TestMethod()]
        public void CreateTest()
        {
            // Act
            _grupoMusical.Create(new Grupomusical() {
                Id = 4,
                Agencia = "Sergipe Cultura",
                Bairro = "Centro",
                Banco = "Bradesco",
                Cep = "42520-000",
                ChavePix = "ForroSergipano@gmail.com",
                ChavePixtipo = "E-mail",
                Cidade = "Lagarto",
                Cnpj = "55.975.926/0001-12",
                Email = "ForroSergipano@gmail.com",
                Estado = "SE",
                Facebook = "Forro Sergipano",
                Instagram = "ForroSergipano",
                Nome = "Forro Sergipano",
                NumeroContaBanco = "69944-2",
                Pais = "Brasil",
                RazaoSocial = "Participação Grupo musical ",
                Rua = "Jose Bezerra",
                Telefone1 = "79999954931",
                Telefone2 = "3443-1879",
                Youtube = "Forro Sergipano",
            });
            // Assert
            Assert.AreEqual(4, _grupoMusical.GetAll().Count());
            var grupoMusical = _grupoMusical.Get(4);
            Assert.AreEqual("Sergipe Cultura", grupoMusical.Agencia);
            Assert.AreEqual("Centro", grupoMusical.Bairro);
            Assert.AreEqual("Bradesco", grupoMusical.Banco);
            Assert.AreEqual("42520-000", grupoMusical.Cep);
            Assert.AreEqual("ForroSergipano@gmail.com", grupoMusical.ChavePix);
            Assert.AreEqual("E-mail", grupoMusical.ChavePixtipo);
            Assert.AreEqual("Lagarto", grupoMusical.Cidade);
            Assert.AreEqual("55.975.926/0001-12", grupoMusical.Cnpj);
            Assert.AreEqual("ForroSergipano@gmail.com", grupoMusical.Email);
            Assert.AreEqual("SE", grupoMusical.Estado);
            Assert.AreEqual("Forro Sergipano", grupoMusical.Facebook);
            Assert.AreEqual("ForroSergipano", grupoMusical.Instagram);
            Assert.AreEqual("Forro Sergipano", grupoMusical.Nome);
            Assert.AreEqual("69944-2", grupoMusical.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusical.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusical.RazaoSocial);
            Assert.AreEqual("Jose Bezerra", grupoMusical.Rua);
            Assert.AreEqual("79999954931", grupoMusical.Telefone1);
            Assert.AreEqual("3443-1879", grupoMusical.Telefone2);
            Assert.AreEqual("Forro Sergipano", grupoMusical.Youtube);
        }

        [TestMethod()]
        public void DeleteTest()
        {
            _grupoMusical.Delete(2);
            // Assert
            Assert.AreEqual(2, _grupoMusical.GetAll().Count());
            var grupomusical = _grupoMusical.Get(2);
            Assert.AreEqual(null, grupomusical);
        }

        [TestMethod()]
        public void EditTest()
        {
            var grupoMusical = _grupoMusical.Get(3);
            grupoMusical.Id = 1;
            grupoMusical.Agencia = "Sergipe Cultura";
            grupoMusical.Bairro = "Bairro";
            grupoMusical.Banco = "Master Card";
            grupoMusical.Cep = "49520-111";
            grupoMusical.ChavePix = "GrupoArraia@gmail.com";
            grupoMusical.ChavePixtipo = "E-mail";
            grupoMusical.Cidade = "São Domingos";
            grupoMusical.Cnpj = "10.875.926/0001-28";
            grupoMusical.Email = "GrupoArraia@gmail.com";
            grupoMusical.Estado = "SE";
            grupoMusical.Facebook = "Grupo Arraia";
            grupoMusical.Instagram = "Grupo Arraia";
            grupoMusical.Nome = "Grupo Arraia";
            grupoMusical.NumeroContaBanco = "78756-2";
            grupoMusical.Pais = "Brasil";
            grupoMusical.RazaoSocial = "Participação Grupo musical ";
            grupoMusical.Rua = "Campo do Brito";
            grupoMusical.Telefone1 = "79999857845";
            grupoMusical.Telefone2 = "3433-7859";
            grupoMusical.Youtube = "Grupo Arraia";
            
            //Assert

            Assert.AreEqual("Sergipe Cultura", grupoMusical.Agencia);
            Assert.AreEqual("Bairro", grupoMusical.Bairro);
            Assert.AreEqual("Master Card", grupoMusical.Banco);
            Assert.AreEqual("49520-111", grupoMusical.Cep);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusical.ChavePix);
            Assert.AreEqual("E-mail", grupoMusical.ChavePixtipo);
            Assert.AreEqual("São Domingos", grupoMusical.Cidade);
            Assert.AreEqual("10.875.926/0001-28", grupoMusical.Cnpj);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusical.Email);
            Assert.AreEqual("SE", grupoMusical.Estado);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Facebook);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Instagram);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Nome);
            Assert.AreEqual("78756-2", grupoMusical.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusical.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusical.RazaoSocial);
            Assert.AreEqual("Campo do Brito", grupoMusical.Rua);
            Assert.AreEqual("79999857845", grupoMusical.Telefone1);
            Assert.AreEqual("3433-7859", grupoMusical.Telefone2);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Youtube);
        }

        [TestMethod()]
        public void GetTest()
        {
            var grupoMusical = _grupoMusical.Get(1);
            Assert.IsNotNull(grupoMusical);
            Assert.AreEqual(1, grupoMusical.Id);
            Assert.AreEqual("Sergipe Cultura", grupoMusical.Agencia);
            Assert.AreEqual("Centro", grupoMusical.Bairro);
            Assert.AreEqual("Banco do Brasil", grupoMusical.Banco);
            Assert.AreEqual("49520-111", grupoMusical.Cep);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusical.ChavePix);
            Assert.AreEqual("E-mail", grupoMusical.ChavePixtipo);
            Assert.AreEqual("Itabaina", grupoMusical.Cidade);
            Assert.AreEqual("10.875.926/0001-20", grupoMusical.Cnpj);
            Assert.AreEqual("GrupoArraia@gmail.com", grupoMusical.Email);
            Assert.AreEqual("SE", grupoMusical.Estado);
            Assert.AreEqual("Grupo Arraia", grupoMusical    .Facebook);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Instagram);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Nome);
            Assert.AreEqual("78756-2", grupoMusical.NumeroContaBanco);
            Assert.AreEqual("Brasil", grupoMusical.Pais);
            Assert.AreEqual("Participação Grupo musical ", grupoMusical.RazaoSocial);
            Assert.AreEqual("Campo do Brito", grupoMusical.Rua);
            Assert.AreEqual("79999857845", grupoMusical.Telefone1);
            Assert.AreEqual("3433-7859", grupoMusical.Telefone2);
            Assert.AreEqual("Grupo Arraia", grupoMusical.Youtube);
        }

        [TestMethod()]
        public void GetAllTest()
        {
            // Act
            var listaGrupoMusical = _grupoMusical.GetAll();
            // Assert
            Assert.IsInstanceOfType(listaGrupoMusical, typeof(IEnumerable<Grupomusical>));
            Assert.IsNotNull(listaGrupoMusical);
            Assert.AreEqual(3, listaGrupoMusical.Count());
            Assert.AreEqual(1, listaGrupoMusical.First().Id);
        }

        [TestMethod]
        public void GetAllDTOTest()
        {
            // Act
            var listaGrupoMusical = _grupoMusical.GetAllDTO();

            // Assert
            Assert.IsInstanceOfType(listaGrupoMusical, typeof(IEnumerable<GrupoMusicalDTO>));
            Assert.IsNotNull(listaGrupoMusical);
            Assert.AreEqual(3, listaGrupoMusical.Count());
            Assert.AreEqual(1, listaGrupoMusical.First().Id);
        }


    }

}