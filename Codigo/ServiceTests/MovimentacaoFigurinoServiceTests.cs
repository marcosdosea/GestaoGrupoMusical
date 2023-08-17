using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestClass]
    public class MovimentacaoFigurinoServiceTests
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoFigurinoService _movimentacaoFigurino;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(warning => warning.Ignore(InMemoryEventId.TransactionIgnoredWarning));
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
                },
                new Pessoa 
                {
                    Id = 3,
                    Cpf = "007.217.424-02",
                    Nome = "Douglas santos",
                    Sexo = "M",
                    Cep = "49520-111",
                    Rua = "Rua 10",
                    Bairro = "Centro",
                    Cidade = "Itabaina",
                    Estado = "SE",
                    DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    Telefone1 = "79998567896",
                    Telefone2 = "79998653284",
                    Email = "Douglassts@gmail.com",
                    DataEntrada = new DateTime(2019, 1, 10, 0, 0, 0, 0, DateTimeKind.Local),
                    DataSaida = new DateTime(2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local),
                    MotivoSaida = "Não me acostumei com a cultura",
                    Ativo = 1,
                    IsentoPagamento = 1,
                    IdGrupoMusical = 1,
                    IdPapelGrupo = 1,
                    IdManequim = 1
                },
                new Pessoa()
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
                    DataNascimento = new DateTime(1998, 2, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    Telefone1 = "79996565896",
                    Telefone2 = "79998453284",
                    Email = "jorgests@gmail.com",
                    DataEntrada = new DateTime(2019, 10, 1, 0, 0, 0, 0, DateTimeKind.Local),
                    DataSaida = new DateTime(2021, 6, 5, 0, 0, 0, 0, DateTimeKind.Local),
                    MotivoSaida = "Discuti com alguns membros",
                    Ativo = 1,
                    IsentoPagamento = 1,
                    IdGrupoMusical = 1,
                    IdPapelGrupo = 1,
                    IdManequim = 1
                }
            };

            _context.AddRange(pessoas);

            var movimentacoesFigurinos = new List<Movimentacaofigurino>
            {
                new Movimentacaofigurino
                {
                    Id = 1,
                    Data = new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 1,
                    IdAssociado = 1,
                    IdColaborador = 2,
                    Status = "DISPONIVEL",
                    ConfirmacaoRecebimento = 0 
                },
                new Movimentacaofigurino
                {
                    Id = 2,
                    Data = new DateTime(2023, 3, 28, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 2,
                    IdAssociado = 2,
                    IdColaborador = 2,
                    Status = "ENTREGUE",
                    ConfirmacaoRecebimento = 1
                },
                new Movimentacaofigurino
                {
                    Id = 3,
                    Data = new DateTime(2023, 4, 2, 0, 0, 0, 0, DateTimeKind.Local),
                    IdFigurino = 3,
                    IdAssociado = 3,
                    IdColaborador = 2,
                    Status = "DEVOLVIDO",
                    ConfirmacaoRecebimento = 1
                }
            };

            _context.AddRange(movimentacoesFigurinos);
            _context.SaveChanges();

            _movimentacaoFigurino = new MovimentacaoFigurinoService(_context);
        }

        [TestMethod]
        public void CreateAsyncTest()
        {
            // Act
            var result = _movimentacaoFigurino.CreateAsync(new Movimentacaofigurino
            {
                Id = 4,
                Data = new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Local),
                IdFigurino = 4,
                IdAssociado = 4,
                IdColaborador = 2,
                Status = "DISPONIVEL",
                ConfirmacaoRecebimento = 0
            }).Result;

            // Assert
            Assert.AreEqual(200, result);
            var movimentacaoFigurino = _context.Movimentacaofigurinos.FindAsync(4).Result;
            
            Assert.IsNotNull(movimentacaoFigurino);
            Assert.AreEqual(4, movimentacaoFigurino.Id);
        }
    }
}
