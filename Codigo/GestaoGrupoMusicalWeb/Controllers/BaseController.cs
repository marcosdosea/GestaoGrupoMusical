using Core;
using Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoGrupoMusicalWeb.Controllers
{
    public abstract class BaseController : Controller
    {
        public enum Notifica
        {
            Sucesso,
            Erro,
            Informativo,
            Alerta
        }
        public void Notificar(string mensagem, Notifica alertType)
        {
            switch(alertType){
                case Notifica.Sucesso:
                    TempData["alertType"] = "success";
                    TempData["alertIcon"] = "fa-solid fa-circle-check";
                break;
                case Notifica.Erro:
                    TempData["alertType"] = "danger";
                    TempData["alertIcon"] = "fa-solid fa-circle-xmark";
                break;
                case Notifica.Informativo:
                    TempData["alertType"] = "info";
                    TempData["alertIcon"] = "fa-solid fa-circle-info";
                break;
                case Notifica.Alerta:
                    TempData["alertType"] = "warning";
                    TempData["alertIcon"] = "fa-solid fa-circle-exclamation";
                break;
            }
            TempData["alertText"] = mensagem;
        }

        public async Task<int> RequestPasswordReset(UserManager<UsuarioIdentity> _userManager, string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            //a segunda condição é para caso seja necessario
            //confirmar o email do usuario para alterar a senha
            if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
            {
                //usuario não encontrado
                return 400;
            }

            string code = "";

            try
            {
                //gera o token para redefinir senha
                code = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch(Exception ex)
            {
                //ero na geração do token
                return 500;
            }

            //gera link para a view da controladora ja passando codigo e id do usuario
            var callbackUrl = Url.Action("ResetPassword", "Identity", new { userId = user.Id, token = code }, /*protocol:*/ Request.Scheme);

            //enviar email com o link
            EmailModel email = new()
            {
                Assunto = "Batalá - Redefinição de Senha",
                Body = "<div style=\"text-align: center;\">\r\n    " +
                "<h1>Redefinição de Senha</h1>\r\n    " +
                $"<h2>Olá, aqui está o link para redefinir sua senha:</h2>\r\n" +
                $"<a href=\"{callbackUrl}\" style=\"font-weight: 600;\">Clique Aqui</a>"
            };

            email.To.Add(userEmail);

            await EmailService.Enviar(email);

            return 200;
        }
    }
}
