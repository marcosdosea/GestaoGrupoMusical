namespace Core.DTO
{
    public class PessoaEnviarEmailDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
    }


    public class PessoaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
    }
}
