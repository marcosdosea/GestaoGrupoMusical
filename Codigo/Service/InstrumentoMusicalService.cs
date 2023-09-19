using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.Data.Common;
using System.Net;

namespace Service
{
    public class InstrumentoMusicalService : IInstrumentoMusicalService
    {
        private readonly GrupoMusicalContext _context;

        public InstrumentoMusicalService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> Create(Instrumentomusical instrumentoMusical)
        {
            if (instrumentoMusical.DataAquisicao > DateTime.Now)
            {
                return HttpStatusCode.PreconditionFailed;
            }
            try
            {
                await _context.AddAsync(instrumentoMusical);
                await _context.SaveChangesAsync();
                return HttpStatusCode.Created;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
            }

        }

        public async Task<HttpStatusCode> Delete(int id)
        {
            var instrumento = await Get(id);
            if(instrumento == null)
            {
                return HttpStatusCode.NotFound;
            }
            try
            {
                var hasMovimentacao = await _context.Movimentacaoinstrumentos.Where(m => m.IdInstrumentoMusical == id).AsNoTracking().AnyAsync();
                if (hasMovimentacao)
                {
                    return HttpStatusCode.PreconditionFailed;
                }
                _context.Remove(instrumento);
                await _context.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> Edit(Instrumentomusical instrumentoMusical)
        {
            try
            {
                _context.Update(instrumentoMusical);
                await _context.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }

        }

        public async Task<Instrumentomusical?> Get(int id)
        {
            return await _context.Instrumentomusicals.FindAsync(id);
        }

        public async Task<IEnumerable<Instrumentomusical>> GetAll()
        {
            return await _context.Instrumentomusicals.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<InstrumentoMusicalDTO>> GetAllDTO(int idGrupo)
        {
            var query = await (from instrumento in _context.Instrumentomusicals join
                               movimentacao in _context.Movimentacaoinstrumentos
                               on instrumento.Id equals movimentacao.IdInstrumentoMusical into intMovi
                               from instrumentoMovi in intMovi.DefaultIfEmpty()
                               where instrumento.IdGrupoMusical == idGrupo
                               orderby instrumentoMovi.Data descending
                               select new InstrumentoMusicalDTO
                               {
                                   Id = instrumento.Id,
                                   Patrimonio = instrumento.Patrimonio,
                                   NomeInstrumento = instrumento.IdTipoInstrumentoNavigation.Nome,
                                   Status = instrumento.Status,
                                   NomeAssociado = instrumentoMovi.IdAssociadoNavigation.Nome
                               }).AsNoTracking().ToListAsync();


            var list = query.DistinctBy(m => m.Patrimonio).OrderBy(m => m.NomeInstrumento);

            foreach (var instrumento in list)
            {
                if (instrumento.Status == "DISPONIVEL")
                {
                    instrumento.NomeAssociado = "";
                }
                instrumento.Status = instrumento.EnumStatus.Single(s => s.Key == instrumento.Status).Value;
            }

            return list;
        }

        public async Task<IEnumerable<Tipoinstrumento>> GetAllTipoInstrumento()
        {
            return await _context.Tipoinstrumentos.AsNoTracking().ToListAsync();
        }

        public async Task<string> GetNomeInstrumento(int id)
        {
            var query = await (from instrumento in _context.Instrumentomusicals
                               where instrumento.Id == id
                               select new { instrumento.IdTipoInstrumentoNavigation.Nome }).AsNoTracking().SingleOrDefaultAsync();

            return query?.Nome ?? "";
        }

        public async Task<InstrumentoMusicalDeleteDTO> GetInstrumentoMusicalDeleteDTO(int id)
        {
            var query = await (from instrumento in _context.Instrumentomusicals join
                               tipoInstrumento in _context.Tipoinstrumentos
                               on instrumento.IdTipoInstrumento equals tipoInstrumento.Id
                               where id == instrumento.Id
                               select new InstrumentoMusicalDeleteDTO
                               {
                                   Patrimonio = instrumento.Patrimonio,
                                   Status = instrumento.Status,
                                   DataAquisicao = instrumento.DataAquisicao, 
                                   NomeInstrumento = tipoInstrumento.Nome,
                               }).AsNoTracking().SingleOrDefaultAsync();
            return query!;
        }

        //public DatatableResponse<Movimentacaoinstrumento> GetDataPage(DatatableRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<DatatableResponse<InstrumentoMusicalDTO>> GetDataPage(DatatableRequest request, int idGrupo)
        {
            var instrumentoMusical = await GetAllDTO(idGrupo);

            var totalRecords = instrumentoMusical.Count();

            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                instrumentoMusical = instrumentoMusical.Where(g => g.Patrimonio.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                                           || g.Status.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                                           || g.NomeAssociado.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                                           || g.NomeInstrumento.ToString().Contains(request.Search.GetValueOrDefault("value")));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    instrumentoMusical = instrumentoMusical.OrderBy(g => g.Patrimonio);
                else
                    instrumentoMusical = instrumentoMusical.OrderByDescending(g => g.Patrimonio);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    instrumentoMusical = instrumentoMusical.OrderBy(g => g.NomeInstrumento);
                else
                    instrumentoMusical = instrumentoMusical.OrderByDescending(g => g.NomeInstrumento);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    instrumentoMusical = instrumentoMusical.OrderBy(g => g.Status);
                else
                    instrumentoMusical = instrumentoMusical.OrderByDescending(g => g.Status);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("3"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    instrumentoMusical = instrumentoMusical.OrderBy(g => g.NomeAssociado);
                else
                    instrumentoMusical = instrumentoMusical.OrderByDescending(g => g.NomeAssociado);
            }

            int countRecordsFiltered = instrumentoMusical.Count();

            instrumentoMusical = instrumentoMusical.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<InstrumentoMusicalDTO>
            {
                Data = instrumentoMusical.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };


        }
    }
}
