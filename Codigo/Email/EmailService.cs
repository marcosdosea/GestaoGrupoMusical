
using System.Net;
using System.Net.Mail;
using Email.Template;

namespace Email
{
    public class EmailService
    {
        public static async Task<bool> Enviar(EmailModel mailModel)
        {
            if (mailModel.To == null || mailModel.To.Count == 0)
            {
                Console.WriteLine("ERRO: Nenhum destinatário informado.");
                return false;
            }

            try
            {
                // Obtém as configurações do .env.
                // Se o modelo já tiver um valor preenchido, ele terá prioridade.
                string host = !string.IsNullOrWhiteSpace(mailModel.Host)
                    ? mailModel.Host
                    : Environment.GetEnvironmentVariable("EMAIL_HOST")
                        ?? "email-ssl.com.br";

                string userName = !string.IsNullOrWhiteSpace(mailModel.UserName)
                    ? mailModel.UserName
                    : Environment.GetEnvironmentVariable("EMAIL_USERNAME")
                        ?? string.Empty;

                string password = !string.IsNullOrWhiteSpace(mailModel.Password)
                    ? mailModel.Password
                    : Environment.GetEnvironmentVariable("EMAIL_PASSWORD")
                        ?? string.Empty;

                string from = !string.IsNullOrWhiteSpace(mailModel.From)
                    ? mailModel.From
                    : Environment.GetEnvironmentVariable("EMAIL_FROM")
                        ?? userName;

                int port = mailModel.Port > 0
                    ? mailModel.Port
                    : int.TryParse(
                        Environment.GetEnvironmentVariable("EMAIL_PORT"),
                        out int portaConfigurada)
                            ? portaConfigurada
                            : 587;

                // Valida as configurações antes de tentar enviar.
                if (string.IsNullOrWhiteSpace(host) ||
                    string.IsNullOrWhiteSpace(userName) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(from))
                {
                    Console.WriteLine(
                        "ERRO: Configurações SMTP ausentes. Verifique o arquivo .env."
                    );

                    return false;
                }

                Console.WriteLine("========== SMTP ==========");
                Console.WriteLine($"Host: {host}");
                Console.WriteLine($"Porta: {port}");
                Console.WriteLine($"Usuário: {userName}");
                Console.WriteLine($"From: {from}");
                Console.WriteLine("SSL: true");
                Console.WriteLine(
                    $"Destinatários: {string.Join(", ", mailModel.To)}"
                );

                using MailMessage mail = new MailMessage();

                mail.From = new MailAddress(from, mailModel.Assunto);

                foreach (var destinatario in mailModel.To)
                {
                    mail.To.Add(destinatario);
                }

                mail.Subject = mailModel.Subject;

                string bodyCenter =
                    "<br><br>" +
                    "<div style=\"padding-top: 20px;padding-bottom: 20px;background-color: #198754;\">" +
                        "<h2 style=\"color: white;text-align: center;\">" +
                            mailModel.Assunto +
                        "</h2>" +
                    "</div>" +

                    "<div style=\"color: black; text-align: left;\">" +

                        "<h3 style=\"text-indent: 10px;\">" +
                            "Olá, " + mailModel.AddresseeName +
                        "</h3>" +

                        "<p style=\"text-indent: 10px; font-size:12pt;\">" +
                            mailModel.Body +
                        "</p>" +

                    "</div>" +

                    "<br><br>";

                mail.Body =
                    EmailResource.EmailPatternTop +
                    bodyCenter +
                    EmailResource.EmailPatternDown;

                mail.IsBodyHtml = true;

                using SmtpClient smtp = new SmtpClient(host, port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(userName, password)
                };

                Console.WriteLine("Conectando ao servidor SMTP...");

                await smtp.SendMailAsync(mail);

                Console.WriteLine(
                    "SUCESSO: O servidor SMTP aceitou o e-mail."
                );

                Console.WriteLine("==========================");

                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("========== ERRO SMTP ==========");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"StatusCode: {ex.StatusCode}");
                Console.WriteLine($"Detalhes: {ex}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"InnerException: {ex.InnerException}"
                    );
                }

                Console.WriteLine("===============================");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== ERRO GERAL ==========");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("================================");

                return false;
            }
        }
    }
}