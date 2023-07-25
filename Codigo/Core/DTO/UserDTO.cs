
namespace Core.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? DataNascimento { get; set; }
        public string Telefone1 { get; set; } = string.Empty;
        public string? Telefone2 { get; set; }
        public string Email { get; set; } = string.Empty;
        public int IdGrupoMusical { get; set; }
        public int IdPapelGrupo { get; set; }
    }
}
