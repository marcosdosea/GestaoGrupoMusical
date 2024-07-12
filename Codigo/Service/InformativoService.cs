using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Service
{
    public class InformativoService : IInformativoService
    {
        private readonly GrupoMusicalContext _context;
        public InformativoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> Create(Informativo informativo)
        {
            try
            {
                await _context.Informativos.AddAsync(informativo);
                await _context.SaveChangesAsync();
                return HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public async Task<HttpStatusCode> Delete(uint id)
        {
            var informativo = await Get(id);
            if (informativo == null)
            {
                return HttpStatusCode.NotFound;
            }
            _context.Remove(informativo);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Edit(Informativo informativo)
        {
            try
            {
                informativo.Data = DateTime.Now;
                _context.Informativos.Update(informativo);
                _context.SaveChanges();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.MethodNotAllowed;
            }
        }

        public async Task<Informativo?> Get(uint id)
        {
            return await _context.Informativos.FindAsync(id);
        }

        public async Task<IEnumerable<Informativo>> GetAll()
        {
            return await _context.Informativos.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<Informativo>> GetAllInformativoServiceIdGrupo(int idGrupoMusical, int idPessoa)
        {
            var query = await (from informativoService in _context.Informativos
                               where informativoService.IdGrupoMusical == idGrupoMusical && informativoService.IdPessoa == idPessoa
                               select informativoService).ToListAsync();

            return query;
        }

        public DatatableResponse<InformativoIndexDTO> GetDataPage(DatatableRequest request, int idGrupo, IEnumerable<InformativoIndexDTO> InformativoIndexDTO)
        {
            var totalRecords = InformativoIndexDTO.Count();
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                InformativoIndexDTO = InformativoIndexDTO.Where(g => g.Mensagem.ToString().Contains(request.Search.GetValueOrDefault("value")!));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                {
                    InformativoIndexDTO = InformativoIndexDTO.OrderByDescending(g => g.Data);
                }
                else
                {
                    InformativoIndexDTO = InformativoIndexDTO.OrderBy(g => g.Data);
                }
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    InformativoIndexDTO = InformativoIndexDTO.OrderBy(g => g.Mensagem);
                else
                    InformativoIndexDTO = InformativoIndexDTO.OrderByDescending(g => g.Mensagem);
            }

            int countRecordsFiltered = InformativoIndexDTO.Count();

            InformativoIndexDTO = InformativoIndexDTO.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<InformativoIndexDTO>
            {
                Data = InformativoIndexDTO.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }


        public async Task<HttpStatusCode> NotificarInformativoViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, uint idInformativo, string mensagem)
        {
            try
            {
                var informativo = await Get(idInformativo);
                if (informativo != null) 
                {
                    List<EmailModel> emailsBody = new List<EmailModel>();
                    foreach (PessoaEnviarEmailDTO p in pessoas)
                    {
                        emailsBody.Add(new EmailModel()
                        {
                            Assunto = $"Batalá - Notificação de Novo Informativo",
                            AddresseeName = p.Nome,
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h3>" + mensagem + "</h3>\r\n</div>",
                            To = new List<string> { p.Email }
                        });
                    }

                    List<Task> emailTask = new List<Task>();
                    foreach(EmailModel email in emailsBody)
                    {
                        emailTask.Add(EmailService.Enviar(email));
                    }

                    return HttpStatusCode.OK;
                }

                return HttpStatusCode.NotFound;
            } 
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

    }
}

