using System.Net.Mail;
using System.Net;
using Email.Template;

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

            //centro do html do email
            string bodyCenter = "<div style=\"padding-top: 2%;padding-bottom: 2%;background-color: #198754;\"" +
                                    "<h2 style=\"color: #FFFFFF;text-align: center;\">" + mailModel.Assunto + "</h2>" +
                                "</div>" +
                                "<div>" +
                                    "<h3 style=\"text-indent: 10px;\">Olá, " + mailModel.AddresseeName + "</h3>" +
                                    "<p style=\"text-indent: 10px;\">" +
                                        mailModel.Body +
                                    "</p>"+
                                "</div>";

            //mail.Body = mailModel.Body;
            mail.Body = EmailResource.EmailPatternTop + bodyCenter + EmailResource.EmailPatternDown;

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