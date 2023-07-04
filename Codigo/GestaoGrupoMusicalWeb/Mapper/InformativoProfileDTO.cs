using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class InformativoProfileDTO : Profile
    {
        public InformativoProfileDTO()
        {
            CreateMap<InformativoViewModelDTO, InformativoDTO>().ReverseMap();
        }
    }
}
