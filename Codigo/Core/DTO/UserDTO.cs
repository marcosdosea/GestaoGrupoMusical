namespace Core.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Papel { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdPapelGrupo { get; set; }
    }
}
