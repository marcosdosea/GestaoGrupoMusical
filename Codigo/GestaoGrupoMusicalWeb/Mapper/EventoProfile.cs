using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EventoProfile : Profile
    {
        public EventoProfile()
        {
            CreateMap<EventoViewModel, Evento>().ReverseMap();
            CreateMap<EventoCreateViewlModel, Evento>().ReverseMap();
        }
    }
}
