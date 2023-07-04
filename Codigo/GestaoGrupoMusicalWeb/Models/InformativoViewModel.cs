namespace GestaoGrupoMusicalWeb.Models
{
    public class InformativoViewModel
    {
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }
        public string Mensagem { get; set; } = null!;
        public DateTime Data { get; set; }
        public sbyte EntregarAssociadosAtivos { get; set; }
    }
}
