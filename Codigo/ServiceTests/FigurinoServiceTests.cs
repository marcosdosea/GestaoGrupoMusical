using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests
{
    [TestClass]
    public class FigurinoServiceTests
    {
        private GrupoMusicalContext _context;
        private IFigurinoService _figurino;

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
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
                    DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    Telefone1 = "79998567896",
                    Telefone2 = "79998653284",
                    Email = "josests@gmail.com",
                    DataEntrada = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    DataSaida = new DateTime (1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
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
                    DataNascimento = new DateTime (1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    Telefone1 = "79956567896",
                    Telefone2 = "79998653654",
                    Email = "matheussts@gmail.com",
                    DataEntrada = new DateTime (2013, 2, 24, 0, 0, 0, 0, DateTimeKind.Local),
                    DataSaida = new DateTime (2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local),
                    MotivoSaida = "Não me acostumei com a cultura",
                    Ativo = 1,
                    IsentoPagamento = 1,
                    IdGrupoMusical = 1,
                    IdPapelGrupo = 1,
                    IdManequim = 1
                }
            };
            _context.AddRange(pessoas);

            var figurinos = new List<Figurino>
            {
                new Figurino
                {
                    Id = 1,
                    Nome = "Galinha Pintadinha",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 2,
                    Nome = "Mickey Mouse",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 3,
                    Nome = "Batman",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
                new Figurino
                {
                    Id = 4,
                    Nome = "Laterna Verde",
                    Data = new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local),
                    IdGrupoMusical = 1
                },
            };
            _context.Figurinos.AddRange(figurinos);

            _context.Figurinos.AddRange(figurinos);

            var manequins = new List<Manequim>
            {
                new Manequim
                {
                    Id = 1,
                    Tamanho = "PP",
                    Descricao = "EXTRA PEQUENO"
                },
                new Manequim
                {
                    Id = 2,
                    Tamanho = "P",
                    Descricao = "PEQUENO"
                },
                new Manequim
                {
                    Id = 3,
                    Tamanho = "M",
                    Descricao = "MÉDIO"
                },
                new Manequim
                {
                    Id = 4,
                    Tamanho = "G",
                    Descricao = "GRANDE"
                }
            };
            _context.Manequims.AddRange(manequins);

            var estoquesDeFigurinos = new List<Figurinomanequim>
            {
                new Figurinomanequim
                {
                    IdFigurino = 1,
                    IdManequim = 1,
                    QuantidadeDisponivel = 10,
                    QuantidadeEntregue = 0
                },
                new Figurinomanequim
                {
                    IdFigurino = 2,
                    IdManequim = 3,
                    QuantidadeDisponivel = 10,
                    QuantidadeEntregue = 0
                },
                new Figurinomanequim
                {
                    IdFigurino = 3,
                    IdManequim = 1,
                    QuantidadeDisponivel = 5,
                    QuantidadeEntregue = 0
                },
                new Figurinomanequim
                {
                    IdFigurino = 4,
                    IdManequim = 4,
                    QuantidadeDisponivel = 8,
                    QuantidadeEntregue = 0
                },
            };
            _context.Figurinomanequims.AddRange(estoquesDeFigurinos);

            _context.SaveChanges();

            _figurino = new FigurinoService(_context);
        }

        [TestMethod]
        public void CreateTest()
        {
            // Act
            _figurino.Create(new Figurino
            {
                Id = 5,
                Nome = "Mulher Maravilha",
                Data = new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Local),
                IdGrupoMusical = 1
            }).Wait();

            // Assert
            var figurino = _figurino.Get(5).Result;
            Assert.IsNotNull(figurino);
            Assert.AreEqual(5, figurino.Id);
            Assert.AreEqual("Mulher Maravilha", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical);
        }

        [TestMethod]
        public void DeleteTest()
        {
            // Act 
            _figurino.Delete(1);

            // Arrange
            var figurino = _context.Figurinos.FindAsync(1).Result;
            Assert.IsNull(figurino); 
        }

        [TestMethod]
        public void EditTest()
        {
            // Act
            var figurino = _figurino.Get(2).Result;
            figurino.Nome = "Flash";
            figurino.Data = new DateTime(2022, 8, 21, 0, 0, 0, 0, DateTimeKind.Local);
            _figurino.Edit(figurino);

            // Assert
            var figurinoEditado = _figurino.Get(2).Result;
            Assert.IsNotNull(figurinoEditado);
            Assert.AreEqual(2, figurinoEditado.Id);
            Assert.AreEqual("Flash", figurinoEditado.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 21, 0, 0, 0, 0, DateTimeKind.Local), figurinoEditado.Data);
        }

        [TestMethod]
        public void GetTest()
        {
            // Act
            var figurino = _figurino.Get(3).Result;

            // Assert
            Assert.IsNotNull(figurino);
            Assert.AreEqual(3, figurino.Id);
            Assert.AreEqual("Batman", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical) ;
        }

        [TestMethod]
        public void GetAllTest()
        {
            // Act
            var figurinos = _figurino.GetAll("007.587.624-02").Result;

            // Assert
            Assert.IsNotNull(figurinos);
            Assert.AreEqual(4, figurinos.Count());
            var figurino = figurinos.FirstOrDefault();
            Assert.IsNotNull(figurino);
            Assert.AreEqual(1, figurino.Id);
            Assert.AreEqual("Galinha Pintadinha", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical);
        }

        [TestMethod]
        public void GetByNameTest()
        {
            // Act
            var figurino = _figurino.GetByName("Batman").Result;

            // Assert
            Assert.IsNotNull(figurino);
            Assert.AreEqual(3, figurino.Id);
            Assert.AreEqual("Batman", figurino.Nome);
            Assert.AreEqual(new DateTime(2022, 8, 14, 0, 0, 0, 0, DateTimeKind.Local), figurino.Data);
            Assert.AreEqual(1, figurino.IdGrupoMusical);
        }

        [TestMethod]
        public void GetAllEstoqueDTOTest() 
        {

        }
    }
}
