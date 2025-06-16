using System.Collections.Generic;

namespace GestaoGrupoMusicalWeb.Models
{
    public class HomeViewModel
    {
        public IEnumerable<EventoViewModel> Evento { get; set; }
        public IEnumerable<EnsaioViewModel> Ensaio { get; set; }
        public IEnumerable<InformativoViewModel> Informativo { get; set; }

        public HomeViewModel()
        {
            Evento = new List<EventoViewModel>();
            Ensaio = new List<EnsaioViewModel>();
            Informativo = new List<InformativoViewModel>();
        }
    }
}