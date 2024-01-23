using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Core.DTO;

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
            _context.AddRange(figurinos);

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
            _context.AddRange(manequins);

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
            _context.AddRange(estoquesDeFigurinos);

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
            var estoque = _context.Figurinomanequims.FindAsync(1, 1).Result;
            _context.Figurinomanequims.Remove(estoque!);
            _context.SaveChangesAsync();
            _figurino.Delete(1).GetAwaiter().GetResult();

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
            var figurinos = _figurino.GetAll(1).Result;

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
            // Act
            var estoques = _figurino.GetAllEstoqueDTO(1).Result;

            // Assert
            Assert.IsNotNull(estoques);
            Assert.AreEqual(1, estoques.Count());
            var estoqueDTO = estoques.First();
            Assert.IsNotNull(estoqueDTO);
            Assert.AreEqual(1, estoqueDTO.IdFigurino);
            Assert.AreEqual(1, estoqueDTO.IdManequim);
            Assert.AreEqual("PP", estoqueDTO.Tamanho);
            Assert.AreEqual(10, estoqueDTO.Disponivel);
            Assert.AreEqual(0, estoqueDTO.Entregues);
        }

        [TestMethod]
        public void CreateEstoqueTest() 
        {
            // Act
            _figurino.CreateEstoque(new Figurinomanequim
            {
                IdFigurino = 1,
                IdManequim = 3,
                QuantidadeDisponivel = 5,
                QuantidadeEntregue = 0
            });

            // Assert
            var estoque = _figurino.GetEstoque(1, 3).Result;
            Assert.IsNotNull(estoque);
            Assert.AreEqual(1, estoque.IdFigurino);
            Assert.AreEqual(3, estoque.IdManequim);
            Assert.AreEqual("M", estoque.Tamanho);
            Assert.AreEqual(5, estoque.Disponivel);
            Assert.AreEqual(0, estoque.Entregues);
        }

        [TestMethod]
        public void DeleteEstoqueTest()
        {
            // Act
            _figurino.DeleteEstoque(1, 1);

            // Assert
            var estoque = _context.Figurinomanequims.FindAsync(1, 1).Result;
            Assert.IsNull(estoque);
        }

        [TestMethod]
        public void EditEstoqueTest()
        {
            // Act
            var estoque = _context.Figurinomanequims.FindAsync(2, 3).Result;
            estoque.QuantidadeEntregue = 1;
            estoque.QuantidadeDisponivel = 9;
            _figurino.EditEstoque(estoque);

            // Assert
            var estoqueEditado = _figurino.GetEstoque(2, 3).Result;
            Assert.IsNotNull(estoqueEditado);
            Assert.AreEqual(2, estoqueEditado.IdFigurino);
            Assert.AreEqual(3, estoqueEditado.IdManequim);
            Assert.AreEqual("M", estoqueEditado.Tamanho);
            Assert.AreEqual(9, estoqueEditado.Disponivel);
            Assert.AreEqual(1, estoqueEditado.Entregues);
        }

        [TestMethod]
        public void GetEstoqueTest()
        {
            // Act
            var estoque = _figurino.GetEstoque(4, 4).Result;

            // Assert
            Assert.IsNotNull(estoque);
            Assert.AreEqual(4, estoque.IdFigurino);
            Assert.AreEqual(4, estoque.IdManequim);
            Assert.AreEqual("G", estoque.Tamanho);
            Assert.AreEqual(8, estoque.Disponivel);
            Assert.AreEqual(0, estoque.Entregues);
        }
    }
}
