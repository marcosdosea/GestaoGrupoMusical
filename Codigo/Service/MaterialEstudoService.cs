using Core;
using Core.DTO;
using Core.Datatables;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Net;
namespace Service
{
    public class MaterialEstudoService : IMaterialEstudoService
    {
        private readonly GrupoMusicalContext _context;
        public MaterialEstudoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(Materialestudo materialEstudo)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {

                await _context.AddAsync(materialEstudo);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<HttpStatusCode> Delete(int id)
        {
            var materialEstudo = await Get(id);
            if (materialEstudo == null)
            {
                return HttpStatusCode.NotFound;
            }
            _context.Remove(materialEstudo);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        public async Task<bool> Edit(Materialestudo materialEstudo)
        {
            try
            {
                _context.Update(materialEstudo);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                return false;
            }
        }

        public async Task<Materialestudo?> Get(int id)
        {
            return await _context.Materialestudos.FindAsync(id);
        }

        public async Task<IEnumerable<Materialestudo>> GetAll()
        {
            return await _context.Materialestudos.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Materialestudo>> GetAllMaterialEstudoPerIdGrupo(int idGrupoMusical)
        {
            var query = await (from materialEstudo in _context.Materialestudos
                               join grupoMusical in _context.Grupomusicals
                               on materialEstudo.IdGrupoMusical equals idGrupoMusical
                               orderby materialEstudo.Nome ascending
                               select new Materialestudo
                               {
                                   Id = materialEstudo.Id,
                                   Nome = materialEstudo.Nome,
                                   Link = materialEstudo.Link,
                                   Data = materialEstudo.Data,
                                   //Data = materialEstudo.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   //g.DataHoraInicio.ToString("dd/MM/yyyy HH:mm:ss"),
                                   //IdGrupoMusical = idGrupoMusical,
                               }).AsNoTracking().ToListAsync();
            return query.ToList();
        }

        public async Task<DatatableResponse<Materialestudo>> GetDataPage(DatatableRequest request, int idGrupo)
        {
            var materiaisEstudo = await GetAllMaterialEstudoPerIdGrupo(idGrupo);

            var totalRecords = materiaisEstudo.Count();

            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                materiaisEstudo = materiaisEstudo.Where(g => g.Nome.ToString().Contains(request.Search.GetValueOrDefault("value"))
                                                           || g.Link.ToString().Contains(request.Search.GetValueOrDefault("value")));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    materiaisEstudo = materiaisEstudo.OrderBy(g => g.Data);
                else
                    materiaisEstudo = materiaisEstudo.OrderByDescending(g => g.Data);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    materiaisEstudo = materiaisEstudo.OrderBy(g => g.Nome);
                else
                    materiaisEstudo = materiaisEstudo.OrderByDescending(g => g.Nome);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column").Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir").Equals("asc"))
                    materiaisEstudo = materiaisEstudo.OrderBy(g => g.Link);
                else
                    materiaisEstudo = materiaisEstudo.OrderByDescending(g => g.Link);
            }

            int countRecordsFiltered = materiaisEstudo.Count();

            materiaisEstudo = materiaisEstudo.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<Materialestudo>
            {
                Data = materiaisEstudo.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };


        }

    }
}
