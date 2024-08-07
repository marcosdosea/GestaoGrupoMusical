using Core;
using Core.DTO;
using Core.Datatables;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Email;
using System.Data;

namespace Service
{
    public class MaterialEstudoService : IMaterialEstudoService
    {
        private readonly GrupoMusicalContext _context;
        public MaterialEstudoService(GrupoMusicalContext context)
        {
            _context = context;
        }

        public async Task<HttpStatusCode> Create(Materialestudo materialEstudo)
        {
            try
            {
                await _context.AddAsync(materialEstudo);
                await _context.SaveChangesAsync();
                return HttpStatusCode.Created;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
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
                materialEstudo.Data = DateTime.Now;
                _context.Update(materialEstudo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
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
                               where materialEstudo.IdGrupoMusical == idGrupoMusical
                               select materialEstudo).ToListAsync();

            return query;
        }

        public DatatableResponse<MaterialEstudoIndexDTO> GetDataPage(DatatableRequest request,IEnumerable <MaterialEstudoIndexDTO> materialEstudoIndexDTO)
        {
            var totalRecords = materialEstudoIndexDTO.Count();
            if (request.Search != null && request.Search.GetValueOrDefault("value") != null)
            {
                materialEstudoIndexDTO = materialEstudoIndexDTO.Where(g => g.Nome.ToString().Contains(request.Search.GetValueOrDefault("value")!));
            }

            if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("0"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                {
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderByDescending(g => g.Data);
                }
                else
                {
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderBy(g => g.Data);
                }
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("1"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderBy(g => g.Nome);
                else
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderByDescending(g => g.Nome);
            }
            else if (request.Order != null && request.Order[0].GetValueOrDefault("column")!.Equals("2"))
            {
                if (request.Order[0].GetValueOrDefault("dir")!.Equals("asc"))
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderBy(g => g.Link);
                else
                    materialEstudoIndexDTO = materialEstudoIndexDTO.OrderByDescending(g => g.Link);
            }

            int countRecordsFiltered = materialEstudoIndexDTO.Count();

            materialEstudoIndexDTO = materialEstudoIndexDTO.Skip(request.Start).Take(request.Length);

            return new DatatableResponse<MaterialEstudoIndexDTO>
            {
                Data = materialEstudoIndexDTO.ToList(),
                Draw = request.Draw,
                RecordsFiltered = countRecordsFiltered,
                RecordsTotal = totalRecords
            };
        }

        public async Task<HttpStatusCode> NotificarMaterialViaEmail(IEnumerable<PessoaEnviarEmailDTO> pessoas, int idMaterialEstudo)
        {
            try
            {
                var materialEstudo = await Get(idMaterialEstudo);
                if (materialEstudo != null)
                {
                    List<EmailModel> emailsBody = new List<EmailModel>();
                    foreach (PessoaEnviarEmailDTO p in pessoas)
                    {
                        emailsBody.Add(new EmailModel()
                        {
                            Assunto = $"Batalá - Notificação de Material de Estudo: {materialEstudo.Nome}",
                            AddresseeName = p.Nome,
                            Body = "<div style=\"text-align: center;\">\r\n    " +
                                $"<h3>Vem dar uma olhada nesse material de estudo! <a style=\"text-decoration: none;\" href=\"{materialEstudo.Link}\">Clique aqui!</a></h3>\r\n</div>",
                            To = new List<string> { p.Email }
                        });
                    }

                    /*
                     * Se cada envio dura 1 segundo (exemplo) e tem 10 emails, se enviar de 1 em 1 
                     * e já que são assícronos, vai acabar demorando 10 segundos. A Task faz com que
                     * todos sejam enviados em paralelo e quando todos já estiverem prontos
                     * ele retorna o controle. Isso quer dizer que os 10 emails enviado podem demorar
                     * 1 segundo. Porém, foi comentada a parte assíncrona porque em um sistema não é
                     * necessário esperar o e-mail chegar até o usuário. Isso vai travar o sistema
                     * até que o e-mail seja enviado. Com a linha comentada, é aquela coisa: "Se
                     * enviar, enviou".
                     */
                    List<Task> emailTask = new List<Task>();
                    foreach (EmailModel ema in emailsBody)
                    {
                        emailTask.Add(EmailService.Enviar(ema));
                    }
                    //await Task.WhenAll(emailTask);

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
