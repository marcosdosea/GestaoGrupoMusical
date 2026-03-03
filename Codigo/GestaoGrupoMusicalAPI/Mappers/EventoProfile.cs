using AutoMapper;
using Core;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mappers
{
    public class EventoProfile : Profile
    {
        public EventoProfile()
        {
            CreateMap<EventoCreateDTO, Evento>()
                .ForMember(dest => dest.Repertorio,
                           opt => opt.MapFrom(_ => string.Empty));

            CreateMap<EventoViewModel, Evento>().ReverseMap();
        }
    }
}
