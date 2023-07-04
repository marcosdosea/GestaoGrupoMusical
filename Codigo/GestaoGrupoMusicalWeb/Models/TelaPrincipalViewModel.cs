namespace GestaoGrupoMusicalWeb.Models
{
    public class TelaPrincipalViewModel
    {
        public IEnumerable<EnsaioViewModelDTO> Ensaio { get; set; }
        public IEnumerable<EventoViewModelDTO> Evento { get; set; }
        public IEnumerable<InformativoViewModelDTO> Informativo { get; set; }

    }
}
