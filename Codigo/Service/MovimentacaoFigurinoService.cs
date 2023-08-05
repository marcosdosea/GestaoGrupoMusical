using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MovimentacaoFigurinoService : IMovimentacaoFigurinoService
    {
        private readonly GrupoMusicalContext _context;

        public MovimentacaoFigurinoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public Task<int> CreateAsync(Movimentacaofigurino movimentacao)
        {
            using var transaction = _context.Database.BeginTransaction();


            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MovimentacaoFigurinoDTO>> GetAllByIdInstrumento(int idFigurino)
        {
            throw new NotImplementedException();
        }

        public Task<Movimentacaofigurino?> GetEmprestimoByIdInstrumento(int idFigurino)
        {
            throw new NotImplementedException();
        }
    }
}
