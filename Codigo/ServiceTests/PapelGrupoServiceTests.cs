using Core.Service;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service.Tests
{
    [TestClass]
    public class PapelGrupoServiceTests
    {
        private GrupoMusicalContext _context;
        private IPapelGrupo _papelGrupo;

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
            var PapeisGrupo = new List<Papelgrupo>
            {
                new Papelgrupo
                {
                    IdPapelGrupo = 1,
                    Nome = "COLABORADOR"
                },
                new Papelgrupo
                {
                    IdPapelGrupo = 2,
                    Nome = "ADMINISTRADOR GRUPO"
                },
                new Papelgrupo
                {
                    IdPapelGrupo = 3,
                    Nome = "ADMINISTRADOR SISTEMA"
                }
            };

            _context.AddRange(PapeisGrupo);
            _context.SaveChanges();

            _papelGrupo = new PapelGrupoService(_context);
        }

    }
}
