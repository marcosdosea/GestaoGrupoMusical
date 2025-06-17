using Core;
using Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        public int IdGrupoMusical { get => Convert.ToInt32(User.FindFirst("IdGrupoMusical")?.Value); }
        public int IdPessoa { get => Convert.ToInt32(User.FindFirst("IdPessoa")?.Value); }
        public string NomeAssociado { get => User.FindFirst("NomeAssociado")?.Value ?? string.Empty; }
        /// <summary>
        /// Este método tem o tralho de gerar um token para
        /// redefinição de senha do usuário identity, gera
        /// uma url para redefinir a senha e encaminha isso
        /// para o email cadastrado (se existir usuario com ele).
        /// </summary>
        /// <param name="_userManager">UserManager do identity</param>
        /// <param name="userEmail">Email do usuario</param>
        /// <returns>200: Sucesso; 400: usuario não encontrado; 500: problema na geração do token</returns>
        public async Task<HttpStatusCode> RequestPasswordReset(UserManager<UsuarioIdentity> _userManager, string userEmail, string userName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            //a segunda condição é para caso seja necessario
            //confirmar o email do usuario para alterar a senha
            if (user == null /*|| !(await _userManager.IsEmailConfirmedAsync(user))*/)
            {
                //usuario não encontrado
                return HttpStatusCode.NotFound;
            }

            string code = "";

            try
            {
                //gera o token para redefinir senha
                code = await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch(Exception ex)
            {
                //erro na geração do token
                return HttpStatusCode.InternalServerError;
            }

            //gera link para a view da controladora ja passando codigo e id do usuario
            var callbackUrl = Url.Action("ResetPassword", "Identity", new { userId = user.Id, token = code }, /*protocol:*/ Request.Scheme);

            //enviar email com o link
            EmailModel email = new()
            {
                Assunto = "Batalá - Redefinição de Senha",
                AddresseeName = userName,
                Body = "Aqui está o link para redefinir sua senha:\r\n" +
                $"<a href=\"{callbackUrl}\"\">Clique Aqui</a>"
            };

            email.To.Add(userEmail);

            await EmailService.Enviar(email);

            return HttpStatusCode.OK;
        }
    }
}
