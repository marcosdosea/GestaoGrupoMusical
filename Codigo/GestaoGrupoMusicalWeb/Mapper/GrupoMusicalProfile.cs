using AutoMapper;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class GrupoMusicalProfile : Profile
    {
        public GrupoMusicalProfile()
        {
            CreateMap<GrupoMusicalViewModel, GrupoMusicalProfile>().ReverseMap();
        }
    }
}
