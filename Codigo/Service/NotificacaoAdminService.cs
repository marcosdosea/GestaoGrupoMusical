using Core.Service;
using FirebaseAdmin.Messaging;

namespace Service
{
    public class NotificacaoAdminService : INotificacaoAdminService
    {
        public async Task Enviar(string token, string titulo, string corpo)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = titulo,
                    Body = corpo,
                },
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
