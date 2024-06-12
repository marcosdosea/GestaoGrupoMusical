using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class MaterialEstudoProfile : Profile
    {
        public MaterialEstudoProfile()
        {
            CreateMap<MaterialEstudoViewModel, Materialestudo>().ReverseMap();
        }
    }
}
