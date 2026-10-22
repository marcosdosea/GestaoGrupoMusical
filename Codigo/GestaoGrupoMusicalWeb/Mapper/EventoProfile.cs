using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EventoProfile : Profile
    {
        public EventoProfile()
        {
            CreateMap<EventoViewModel, Evento>().ReverseMap();
            CreateMap<EventoCreateViewlModel, Evento>()
                .ForMember(dest => dest.PrecisaConfirmarParticipacao,
                    opt => opt.MapFrom(src => src.PrecisaConfirmarParticipacao ? (sbyte)1 : (sbyte)0))
                .ReverseMap()
                .ForMember(dest => dest.PrecisaConfirmarParticipacao,
                    opt => opt.MapFrom(src => src.PrecisaConfirmarParticipacao == 1));
            CreateMap<GerenciarSolicitacaoEventoViewModel, GerenciarSolicitacaoEventoDTO>().ReverseMap();
            CreateMap<EventosEnsaiosAssociadoViewlModel, EventosEnsaiosAssociadoDTO>().ReverseMap();
        }
    }
}
