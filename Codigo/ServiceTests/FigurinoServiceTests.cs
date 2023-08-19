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

        [ClassInitialize]
        public void Initialize()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<GrupoMusicalContext>();
            builder.UseInMemoryDatabase("GrupoMusical");
            var options = builder.Options;

            _context = new GrupoMusicalContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

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
            _context.SaveChanges();

            _figurino = new FigurinoService(_context);
        }
    }
}
