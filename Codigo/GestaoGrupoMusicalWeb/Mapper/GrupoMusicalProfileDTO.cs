using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class GrupoMusicalProfileDTO : Profile
    {
        GrupoMusicalProfileDTO()
        {
            CreateMap<GrupoMusicalViewModelDTO, GrupoMusicalDTO>().ReverseMap();
        }
    }
}
