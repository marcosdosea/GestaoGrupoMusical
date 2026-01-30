using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mapper
{
    public class EventoProfile : Profile
    {
        public EventoProfile()
        {
            CreateMap<EventoViewModel, Evento>().ReverseMap();
            CreateMap<EventoCreateViewlModel, Evento>().ReverseMap();
            CreateMap<EventoCreateDTO, Evento>()
                .ForMember(dest => dest.Repertorio, opt => opt.MapFrom(src => string.Empty))
                .ReverseMap();
            CreateMap<GerenciarSolicitacaoEventoViewModel, GerenciarSolicitacaoEventoDTO>().ReverseMap();
            CreateMap<EventosEnsaiosAssociadoViewlModel, EventosEnsaiosAssociadoDTO>().ReverseMap();
        }
    }
}
