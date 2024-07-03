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

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }
        [Display(Name = "Somente Associados Ativos")]
        public sbyte EntregarAssociadosAtivos { get; set; }
    }
}
