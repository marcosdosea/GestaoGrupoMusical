using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class GrupoMusicalProfileDTO : Profile
    {
        public GrupoMusicalProfileDTO()
        {
            CreateMap<GrupoMusicalViewModelDTO, GrupoMusicalDTO>().ReverseMap();
        }
    }
}
