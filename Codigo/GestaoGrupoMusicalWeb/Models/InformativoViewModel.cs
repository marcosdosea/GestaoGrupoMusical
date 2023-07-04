using MessagePack;
using Microsoft.AspNetCore.Cors;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class InformativoViewModel
    {
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }
        [Display(Name ="Informativo")]
        [MaxLength(2000, ErrorMessage = "Ultrapassou o limíte de 2000 caracteres")]
        public string Mensagem { get; set; } = null!;
        [Display(Name = "Data")]
        [Required(ErrorMessage = "campo {0} é obrigatório")]
        public DateTime Data { get; set; }
        public sbyte EntregarAssociadosAtivos { get; set; }
    }
}
