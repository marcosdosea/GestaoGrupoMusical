using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;
using System.Net;

namespace ServiceTests
{
    [TestClass]
    public class EventoHorarioChegadaTests
    {
        private GrupoMusicalContext _context = null!;
        private EventoService _eventoService = null!;
        private DateTime _inicio;

        [TestInitialize]
        public void Initialize()
        {
            // Configuração do banco de dados em memória
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusicalHorarioChegada").ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _inicio = DateTime.Today.AddDays(10).AddHours(18);
            _context.Eventos.Add(new Evento
            {
                Id = 1,
                IdGrupoMusical = 1,
                DataHoraInicio = _inicio,
                DataHoraFim = _inicio.AddHours(2),
                DataHoraChegadaEvento = _inicio.AddHours(-1),
                ObservacaoHoraChegada = "Chegar com o figurino completo.",
                IdColaboradorResponsavel = 1
            });
            _context.SaveChanges();

            _eventoService = new EventoService(_context);
        }

        private Evento NovoEvento(DateTime? chegada) => new()
        {
            IdGrupoMusical = 1,
            DataHoraInicio = _inicio,
            DataHoraFim = _inicio.AddHours(2),
            DataHoraChegadaEvento = chegada,
            IdColaboradorResponsavel = 1
        };

        [TestMethod]
        public async Task Create_ChegadaDepoisDoInicio_DeveRetornarUnprocessableEntity()
        {
            var resultado = await _eventoService.Create(NovoEvento(_inicio.AddMinutes(30)), [], 1);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, resultado);
            Assert.AreEqual(1, _context.Eventos.Count());
        }

        [TestMethod]
        public void Edit_ChegadaDepoisDoInicio_DeveRetornarUnprocessableEntity()
        {
            var evento = NovoEvento(_inicio.AddMinutes(1));
            evento.Id = 1;

            var resultado = _eventoService.Edit(evento, [], 1);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, resultado);
        }

        [TestMethod]
        public void GetDetails_DeveRetornarHorarioEObservacaoDeChegada()
        {
            var detalhes = _eventoService.GetDetails(1);

            Assert.IsNotNull(detalhes);
            Assert.AreEqual(_inicio.AddHours(-1), detalhes.DataHoraChegadaEvento);
            Assert.AreEqual("Chegar com o figurino completo.", detalhes.ObservacaoHoraChegada);
        }
    }
}
