using Core;
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

        public HttpStatusCode Delete(uint id)
        {
            var informativo = Get(id);
            if (informativo == null)
            {
                return HttpStatusCode.NotFound;
            }
            _context.Remove(informativo);
            _context.SaveChanges();
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

