using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class GrupoMusicalProfile : Profile
    {
        public GrupoMusicalProfile()
        {
            CreateMap<GrupoMusicalViewModel, Grupomusical>().ReverseMap();
        }
    }
}
