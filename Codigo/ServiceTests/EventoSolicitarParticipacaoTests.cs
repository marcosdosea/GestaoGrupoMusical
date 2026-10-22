using Core;
using Core.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Net;

namespace ServiceTests
{
    [TestClass]
    public class EventoSolicitarParticipacaoTests
    {
        private const int ID_EVENTO_SEM_CONFIRMACAO = 1;
        private const int ID_EVENTO_COM_CONFIRMACAO = 2;
        private const int ID_INSTRUMENTO = 10;
        private const int ID_ASSOCIADO = 100;

        private GrupoMusicalContext _context = null!;
        private EventoService _eventoService = null!;

        [TestInitialize]
        public void Initialize()
        {
            // Configuração do banco de dados em memória
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusicalSolicitarParticipacao").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Eventos.AddRange(
                new Evento
                {
                    Id = ID_EVENTO_SEM_CONFIRMACAO,
                    IdGrupoMusical = 1,
                    DataHoraInicio = DateTime.Today.AddDays(10),
                    DataHoraFim = DateTime.Today.AddDays(10).AddHours(2),
                    IdColaboradorResponsavel = 1,
                    PrecisaConfirmarParticipacao = 0
                },
                new Evento
                {
                    Id = ID_EVENTO_COM_CONFIRMACAO,
                    IdGrupoMusical = 1,
                    DataHoraInicio = DateTime.Today.AddDays(10),
                    DataHoraFim = DateTime.Today.AddDays(10).AddHours(2),
                    IdColaboradorResponsavel = 1,
                    PrecisaConfirmarParticipacao = 1
                });

            _context.Apresentacaotipoinstrumentos.AddRange(
                new Apresentacaotipoinstrumento
                {
                    IdApresentacao = ID_EVENTO_SEM_CONFIRMACAO,
                    IdTipoInstrumento = ID_INSTRUMENTO,
                    QuantidadePlanejada = 1
                },
                new Apresentacaotipoinstrumento
                {
                    IdApresentacao = ID_EVENTO_COM_CONFIRMACAO,
                    IdTipoInstrumento = ID_INSTRUMENTO,
                    QuantidadePlanejada = 1
                });

            _context.SaveChanges();

            _eventoService = new EventoService(_context);
        }

        [TestMethod]
        public async Task SolicitarParticipacao_EventoSemConfirmacao_DeveDeferirAutomaticamente()
        {
            var resultado = await _eventoService.SolicitarParticipacao(ID_EVENTO_SEM_CONFIRMACAO, ID_ASSOCIADO, ID_INSTRUMENTO);

            Assert.AreEqual(HttpStatusCode.OK, resultado);
            var inscricao = _context.Eventopessoas.Single(ep => ep.IdEvento == ID_EVENTO_SEM_CONFIRMACAO && ep.IdPessoa == ID_ASSOCIADO);
            Assert.AreEqual(InscricaoEventoPessoa.DEFERIDO.ToString(), inscricao.Status);
            var vaga = _context.Apresentacaotipoinstrumentos.Single(a => a.IdApresentacao == ID_EVENTO_SEM_CONFIRMACAO);
            Assert.AreEqual(1, vaga.QuantidadeSolicitada);
            Assert.AreEqual(1, vaga.QuantidadeConfirmada);
        }

        [TestMethod]
        public async Task SolicitarParticipacao_EventoComConfirmacao_DeveFicarInscrito()
        {
            var resultado = await _eventoService.SolicitarParticipacao(ID_EVENTO_COM_CONFIRMACAO, ID_ASSOCIADO, ID_INSTRUMENTO);

            Assert.AreEqual(HttpStatusCode.OK, resultado);
            var inscricao = _context.Eventopessoas.Single(ep => ep.IdEvento == ID_EVENTO_COM_CONFIRMACAO && ep.IdPessoa == ID_ASSOCIADO);
            Assert.AreEqual(InscricaoEventoPessoa.INSCRITO.ToString(), inscricao.Status);
            var vaga = _context.Apresentacaotipoinstrumentos.Single(a => a.IdApresentacao == ID_EVENTO_COM_CONFIRMACAO);
            Assert.AreEqual(1, vaga.QuantidadeSolicitada);
            Assert.AreEqual(0, vaga.QuantidadeConfirmada);
        }

        [TestMethod]
        public async Task SolicitarParticipacao_EventoSemConfirmacaoSemVagas_DeveRetornarBadRequest()
        {
            await _eventoService.SolicitarParticipacao(ID_EVENTO_SEM_CONFIRMACAO, ID_ASSOCIADO, ID_INSTRUMENTO);

            var resultado = await _eventoService.SolicitarParticipacao(ID_EVENTO_SEM_CONFIRMACAO, ID_ASSOCIADO + 1, ID_INSTRUMENTO);

            Assert.AreEqual(HttpStatusCode.BadRequest, resultado);
        }

        [TestMethod]
        public async Task SolicitarParticipacao_EventoComConfirmacaoSemVagasConfirmadas_DeveAceitarInscricao()
        {
            // Com confirmação manual, a vaga só é ocupada quando o administrador defere a inscrição
            await _eventoService.SolicitarParticipacao(ID_EVENTO_COM_CONFIRMACAO, ID_ASSOCIADO, ID_INSTRUMENTO);

            var resultado = await _eventoService.SolicitarParticipacao(ID_EVENTO_COM_CONFIRMACAO, ID_ASSOCIADO + 1, ID_INSTRUMENTO);

            Assert.AreEqual(HttpStatusCode.OK, resultado);
            Assert.AreEqual(2, _context.Eventopessoas.Count(ep => ep.IdEvento == ID_EVENTO_COM_CONFIRMACAO &&
                                                                  ep.Status == InscricaoEventoPessoa.INSCRITO.ToString()));
        }
    }
}
