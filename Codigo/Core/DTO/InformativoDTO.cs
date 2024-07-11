using System.ComponentModel.DataAnnotations;


namespace Core.DTO
{
    public class InformativoIndexDTO
    {
        [Key]
        public int Id { get; set; }
        public DateTime? Data { get; set; }
  
        public string Mensagem { get; set; } = null!;

    }
}



