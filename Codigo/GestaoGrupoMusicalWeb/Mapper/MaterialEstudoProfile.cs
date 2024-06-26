using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class MaterialEstudoProfile : Profile
    {
        public MaterialEstudoProfile()
        {
            CreateMap<MaterialEstudoViewModel, Materialestudo>().ReverseMap();
            CreateMap<MaterialEstudoIndexDTO, Materialestudo>().ReverseMap();
        }
    }
}
