using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public interface INotificacaoAdminService
    {
        Task Enviar(string token, string titulo, string corpo);
    }
}
