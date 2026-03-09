using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWebAPI.Models;

namespace GestaoGrupoMusicalAPI.Mappers
{
    public class MaterialEstudoProfile : Profile
    {
        public MaterialEstudoProfile()
        {
            CreateMap<MaterialEstudoViewModel,Materialestudo>().ReverseMap();
            CreateMap<MaterialEstudoIndexDTO, Materialestudo>().ReverseMap();
        }
    }
}
