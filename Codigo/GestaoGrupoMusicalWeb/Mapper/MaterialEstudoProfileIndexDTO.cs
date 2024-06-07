using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class MaterialEstudoProfileIndexDTO : Profile
    {
        public MaterialEstudoProfileIndexDTO()
        {
            CreateMap<MaterialEstudoViewModelIndexDTO, MaterialEstudoDTO>().ReverseMap();
        }
    }
}
