using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Net;

namespace ServiceTests
{
    [TestClass]
    public class InformativoServiceTests
    {
        private GrupoMusicalContext _context;
        private IInformativoService _informativoService;

        [TestInitialize]
        public void Initialize()
        {
            // Configuração do banco de dados em memória
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Dados iniciais para os testes
            var informativos = new List<Informativo>
            {
                new Informativo
                {
                    Id = 1, // Adicionando IDs para os testes de Get/Delete
                    IdGrupoMusical = 1,
                    IdPessoa = 3,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 6, 18, 8, 0, 0),
                    EntregarAssociadosAtivos = 1,
                },
                new Informativo
                {
                    Id = 2,
                    IdGrupoMusical = 1,
                    IdPessoa = 2,
                    Mensagem = "Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 06, 19, 14, 0, 0),
                    EntregarAssociadosAtivos = 1,
                },
                new Informativo
                {
                    Id = 3,
                    IdGrupoMusical = 1,
                    IdPessoa = 4,
                    Mensagem = "Bom dia! lembrem-se o horário combinado.",
                    Data = new DateTime(2024, 6, 19, 8, 0, 0),
                    EntregarAssociadosAtivos = 1,
                }
            };
            _context.AddRange(informativos);
            _context.SaveChanges();

            _informativoService = new InformativoService(_context);
        }

        [TestMethod]
        public async Task CreateTest()
        {
            // Arrange: Novo informativo com uma chave que ainda não existe
            var novoInformativo = new Informativo
            {
                IdGrupoMusical = 1,
                IdPessoa = 5, // Usando uma chave nova
                Mensagem = "Informativo de teste para o método Create.",
                Data = new DateTime(2024, 06, 30, 14, 0, 0),
                EntregarAssociadosAtivos = 1
            };

            // Act: Chama o método Create do serviço
            var resultado = await _informativoService.Create(novoInformativo);

            // Assert: Verifica se o informativo foi criado com sucesso
            Assert.AreEqual(HttpStatusCode.Created, resultado, "O serviço deve retornar status Created.");

            var informativoCriado = (await _informativoService.GetAllInformativoServiceIdGrupo(1, 5)).FirstOrDefault();
            Assert.IsNotNull(informativoCriado, "O informativo deveria ter sido encontrado no banco.");
            Assert.AreEqual("Informativo de teste para o método Create.", informativoCriado.Mensagem);
        }

        [TestMethod]
        public async Task DeleteTest()
        {
            // Arrange: Pega o ID do informativo que será deletado
            uint idParaDeletar = 2;

            // Act: Chama o método Delete do serviço
            var resultado = await _informativoService.Delete(idParaDeletar);

            // Assert: Verifica se o informativo foi removido
            Assert.AreEqual(HttpStatusCode.OK, resultado, "O serviço deve retornar status OK.");

            var informativoDeletado = await _informativoService.Get(idParaDeletar);
            Assert.IsNull(informativoDeletado, "O informativo não deveria mais ser encontrado após a exclusão.");
        }

        [TestMethod]
        public async Task EditTest()
        {
            // Arrange: Encontra o informativo para editar
            var informativo = (await _informativoService.GetAllInformativoServiceIdGrupo(1, 2)).FirstOrDefault();
            Assert.IsNotNull(informativo, "O informativo a ser editado não foi encontrado.");

            // Modifica a mensagem
            informativo.Mensagem = "Mensagem alterada pelo teste de edição.";

            // Act: Chama o método Edit do serviço
            var resultado = _informativoService.Edit(informativo);

            // Assert: Verifica se a edição foi bem-sucedida
            Assert.AreEqual(HttpStatusCode.OK, resultado, "O serviço deve retornar status OK.");

            var informativoEditado = (await _informativoService.GetAllInformativoServiceIdGrupo(1, 2)).FirstOrDefault();
            Assert.IsNotNull(informativoEditado, "O informativo editado não foi encontrado.");
            Assert.AreEqual("Mensagem alterada pelo teste de edição.", informativoEditado.Mensagem, "A mensagem do informativo deveria ter sido atualizada.");
        }

        [TestMethod]
        public async Task GetTest()
        {
            // Act: Usa o método correto para buscar por IdGrupoMusical e IdPessoa
            var informativo = (await _informativoService.GetAllInformativoServiceIdGrupo(1, 2)).FirstOrDefault();

            // Assert: Verifica se o informativo correto foi retornado
            Assert.IsNotNull(informativo, "O informativo não deveria ser nulo.");
            Assert.AreEqual(1, informativo.IdGrupoMusical);
            Assert.AreEqual(2, informativo.IdPessoa);
            Assert.AreEqual("Bom dia! Hoje acontecerá nossa apresentação, lembrem-se o horário combinado.", informativo.Mensagem);
            Assert.AreEqual(new DateTime(2024, 06, 19, 14, 0, 0), informativo.Data);
            Assert.AreEqual(1, informativo.EntregarAssociadosAtivos);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            // Act
            var informativos = await _informativoService.GetAll();

            // Assert
            Assert.AreEqual(3, informativos.Count());
        }
    }
}