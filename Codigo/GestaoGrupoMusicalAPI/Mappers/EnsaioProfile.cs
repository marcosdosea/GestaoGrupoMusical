using AutoMapper;
using Core;
using Core.DTO;

public class EnsaioProfile : Profile
{
    public EnsaioProfile()
    {
        CreateMap<EnsaioDetailsDTO, EnsaioAssociadoDTO>()
            .ForMember(dest => dest.Inicio, opt => opt.MapFrom(src => src.DataHoraInicio))
            .ForMember(dest => dest.Fim, opt => opt.MapFrom(src => src.DataHoraFim))
            .ForMember(dest => dest.Presente, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.JustificativaAceita, opt => opt.MapFrom(_ => false));

        CreateMap<EnsaioAssociadoDTO, EventosEnsaiosAssociadoDTO>()
            .ForMember(dest => dest.Ensaios, opt => opt.MapFrom(src => new List<EnsaioAssociadoDTO> { src }))
            .ForMember(dest => dest.Eventos, opt => opt.MapFrom(_ => new List<EventoAssociadoDTO>()));
    }
}