using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EnsaioProfileDTO : Profile
    {
        public EnsaioProfileDTO()
        {
            CreateMap<EnsaioViewModelDTO, EnsaioDTO>().ReverseMap();
        }
    }
}
