using System.Net.Mail;
using System.Net;

namespace Email
{
    public class EmailService
    {
        public static async Task<bool> Enviar(EmailModel mailModel)
        {
            if (mailModel.To.Count == 0)
            {
                return false;
            }
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(mailModel.From, mailModel.Assunto);
            foreach (var t in mailModel.To)
            {
                mail.To.Add(t);
            }

            mail.Subject = mailModel.Subject;
            mail.Body = mailModel.Body;
            mail.IsBodyHtml = true;

            using (SmtpClient smtp = new())
            {

                smtp.Host = mailModel.Host;
                smtp.Credentials = new NetworkCredential(mailModel.UserName, mailModel.Password);
                smtp.EnableSsl = true;
                smtp.Port = mailModel.Port;


                await smtp.SendMailAsync(mail);

                return true;
            }
        }
    }
}