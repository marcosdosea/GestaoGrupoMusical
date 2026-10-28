using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service;

namespace ServiceTests
{
    [TestClass]
    public class FinanceiroServiceTests
    {
        private GrupoMusicalContext _context = null!;
        private FinanceiroService _financeiroService = null!;
        private int _idReceita;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusicalFinanceiro").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            _context = new GrupoMusicalContext(builder.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Associados ativos do grupo 1: pagante, em aberto, sem registro, isento sem registro, isento com registro, comprovante
            // Inativo: não deve aparecer em nenhum filtro
            _context.Pessoas.AddRange(
                NovaPessoa(1, isento: false),
                NovaPessoa(2, isento: false),
                NovaPessoa(3, isento: false),
                NovaPessoa(4, isento: true),
                NovaPessoa(5, isento: true),
                NovaPessoa(6, isento: false),
                NovaPessoa(7, isento: false, ativo: false));

            var receitaNoPrazo = new Receitafinanceira
            {
                Id = 1,
                IdGrupoMusical = 1,
                Descricao = "Mensalidade no prazo",
                DataInicio = DateTime.Today.AddDays(-5),
                DataFim = DateTime.Today.AddDays(10),
                Valor = 100
            };
            _idReceita = receitaNoPrazo.Id;

            // Receita vencida: usada para validar a contagem de atrasos
            var receitaVencida = new Receitafinanceira
            {
                Id = 2,
                IdGrupoMusical = 1,
                Descricao = "Mensalidade vencida",
                DataInicio = DateTime.Today.AddDays(-30),
                DataFim = DateTime.Today.AddDays(-5),
                Valor = 100
            };

            _context.Receitafinanceiras.AddRange(receitaNoPrazo, receitaVencida);
            _context.Receitafinanceirapessoas.AddRange(
                NovoPagamento(1, receitaNoPrazo.Id, StatusPagamento.PAGO, dataPagamento: DateTime.Today),
                NovoPagamento(2, receitaNoPrazo.Id, "ABERTO"),
                NovoPagamento(4, receitaNoPrazo.Id, StatusPagamento.ISENTO),
                NovoPagamento(6, receitaNoPrazo.Id, StatusPagamento.PAGO_COMPROVANTE, dataPagamento: DateTime.Today),

                NovoPagamento(2, receitaVencida.Id, "ABERTO"),
                NovoPagamento(3, receitaVencida.Id, StatusPagamento.NAO_PAGOU),
                NovoPagamento(4, receitaVencida.Id, StatusPagamento.ISENTO),
                NovoPagamento(6, receitaVencida.Id, StatusPagamento.PAGO_COMPROVANTE));

            _context.SaveChanges();

            _financeiroService = new FinanceiroService(_context, new Mock<IDispositivoService>().Object);
        }

        private static Pessoa NovaPessoa(int id, bool isento, bool ativo = true) => new()
        {
            Id = id,
            Nome = $"Associado {id}",
            Cpf = $"000.000.000-0{id}",
            Cep = "00000-000",
            Email = $"associado{id}@teste.com",
            Estado = "SE",
            Sexo = "M",
            Telefone1 = "(79) 99999-0000",
            Ativo = (sbyte)(ativo ? 1 : 0),
            IsentoPagamento = (sbyte)(isento ? 1 : 0),
            IdGrupoMusical = 1,
            IdPapelGrupo = 1
        };

        private static Receitafinanceirapessoa NovoPagamento(int idPessoa, int idReceita, string status, DateTime? dataPagamento = null) => new()
        {
            IdPessoa = idPessoa,
            IdReceitaFinanceira = idReceita,
            Valor = 100,
            Status = status,
            DataPagamento = dataPagamento
        };

        private async Task<List<int>> IdsDoFiltro(SituacaoPagamentoFiltro filtro)
        {
            var associados = await _financeiroService.GetAssociadosPagamento(_idReceita, filtro);
            return associados.Select(a => a.IdAssociado).OrderBy(id => id).ToList();
        }

        [TestMethod]
        public async Task GetAssociados_FiltroTodos_DeveRetornarTodosOsAssociadosAtivos()
        {
            var ids = await IdsDoFiltro(SituacaoPagamentoFiltro.Todos);

            CollectionAssert.AreEqual(new List<int> { 1, 2, 3, 4, 5, 6 }, ids);
        }

        [TestMethod]
        public async Task GetAssociados_FiltroPagos_DeveRetornarSomenteQuemPagou()
        {
            var ids = await IdsDoFiltro(SituacaoPagamentoFiltro.Pagos);

            CollectionAssert.AreEqual(new List<int> { 1, 6 }, ids);
        }

        [TestMethod]
        public async Task GetAssociados_FiltroPendentes_DeveIncluirSemRegistroEExcluirIsentos()
        {
            var ids = await IdsDoFiltro(SituacaoPagamentoFiltro.Pendentes);

            CollectionAssert.AreEqual(new List<int> { 2, 3 }, ids);
        }

        [TestMethod]
        public async Task GetAssociados_FiltroIsentos_DeveRetornarIsentosComOuSemRegistro()
        {
            var ids = await IdsDoFiltro(SituacaoPagamentoFiltro.Isentos);

            CollectionAssert.AreEqual(new List<int> { 4, 5 }, ids);
        }

        [TestMethod]
        public async Task GetAssociados_IsentoSemRegistro_DeveTerStatusIsentoENaoNaoPagou()
        {
            var associados = await _financeiroService.GetAssociadosPagamento(_idReceita, SituacaoPagamentoFiltro.Isentos);
            var isentoSemRegistro = associados.Single(a => a.IdAssociado == 5);

            Assert.AreEqual(StatusPagamento.ISENTO, isentoSemRegistro.Status);
        }

        [TestMethod]
        public async Task GetAssociados_PendenteSemRegistro_DeveTerStatusNaoPagou()
        {
            var associados = await _financeiroService.GetAssociadosPagamento(_idReceita, SituacaoPagamentoFiltro.Pendentes);
            var pendenteSemRegistro = associados.Single(a => a.IdAssociado == 3);

            Assert.AreEqual(StatusPagamento.NAO_PAGOU, pendenteSemRegistro.Status);
        }

        [TestMethod]
        public void GetAllFinanceiroPorIdGrupo_Atrasos_DeveContarPendentesVencidosENaoIsentos()
        {
            var receitas = _financeiroService.GetAllFinanceiroPorIdGrupo(1).ToList();

            var noPrazo = receitas.Single(r => r.Id == 1);
            var vencida = receitas.Single(r => r.Id == 2);

            // Receita no prazo: nada está atrasado
            Assert.AreEqual(0, noPrazo.Atrasos);
            Assert.AreEqual(2, noPrazo.Pagos);
            Assert.AreEqual(1, noPrazo.Isentos);

            // Receita vencida: ABERTO e NAO_PAGOU são atrasos; ISENTO e pago sem data não são
            Assert.AreEqual(2, vencida.Atrasos);
            Assert.AreEqual(1, vencida.Isentos);
        }
    }
}
