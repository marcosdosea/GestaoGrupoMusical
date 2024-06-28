using MessagePack;
using Microsoft.AspNetCore.Cors;
using System.ComponentModel.DataAnnotations;

namespace GestaoGrupoMusicalWeb.Models
{
    public class InformativoViewModel
    {
        public uint Id { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }

        [Display(Name ="Mensagem")]
        [Required(ErrorMessage = "A mensagem não pode estar vazia.")]
        [MaxLength(2000, ErrorMessage = "Ultrapassou o limite de 2000 caracteres.")] 
        public string Mensagem { get; set; } = null!;

        [Display(Name = "Data")]
        public DateTime Data { get; set; }
        [Display(Name = "Somente Associados Ativos")]
        public sbyte EntregarAssociadosAtivos { get; set; }
    }
}
