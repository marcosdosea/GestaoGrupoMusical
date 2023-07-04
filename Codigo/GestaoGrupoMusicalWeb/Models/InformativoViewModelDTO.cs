namespace GestaoGrupoMusicalWeb.Models
{
    public class InformativoViewModelDTO
    {
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }
        public string Mensagem { get; set; } = null!;
        public DateTime Data { get; set; }
    }
}
