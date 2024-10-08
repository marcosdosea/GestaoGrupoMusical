using Core;
using Core.Datatables;
using Core.DTO;
using Core.Service;
using Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        public IEnumerable<Informativo> GetAllInformativoServicePorIdGrupoMusical(int idGrupoMusical)
        {
            var query = (from informativo in _context.Informativos
                               where informativo.IdGrupoMusical == idGrupoMusical
                               select new Informativo()
                               {
                                   Id = informativo.Id,
                                   Data = informativo.Data,
                                   Mensagem = informativo.Mensagem,
                               }).ToList();
            return query;
        }

        public DatatableResponse<InformativoIndexDTO> GetDataPage(DatatableRequest request, IEnumerable<InformativoIndexDTO> listaInformativoDTO)
        {
            var totalRecords = listaInformativoDTO.Count();
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                listaInformativoDTO = listaInformativoDTO.Where(g => g.Mensagem.ToString().Contains(request.Search.GetValueOrDefault("value")!, StringComparison.OrdinalIgnoreCase)
                                                           || g.Data.ToString().Contains(request.Search.GetValueOrDefault("value")!));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                {
                    listaInformativoDTO = listaInformativoDTO.OrderByDescending(g => g.Data);
                }
                else
                {
                    listaInformativoDTO = listaInformativoDTO.OrderBy(g => g.Data);
                }
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    listaInformativoDTO = listaInformativoDTO.OrderBy(g => g.Mensagem);
                else
                    listaInformativoDTO = listaInformativoDTO.OrderByDescending(g => g.Mensagem);
            }

            int countRecordsFiltered = listaInformativoDTO.Count();

            listaInformativoDTO = listaInformativoDTO.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<InformativoIndexDTO>
            {
                Data = listaInformativoDTO.ToList(),
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

