using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestClass]
    internal class MovimentacaoFigurinoServiceTests
    {
        private GrupoMusicalContext _context;
        private IMovimentacaoFigurinoService movimentacaoFigurino;

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

            var movimentacoesFigurinos = new List<Movimentacaofigurino>
            {
                new Movimentacaofigurino
                {

                }
            };
        }
    }
}
