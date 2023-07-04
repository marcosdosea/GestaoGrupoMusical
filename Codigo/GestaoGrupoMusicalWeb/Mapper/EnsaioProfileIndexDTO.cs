using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EnsaioProfileIndexDTO: Profile
    {
        public EnsaioProfileIndexDTO()
        {
            CreateMap<EnsaioViewModelIndexDTO, EnsaioIndexDTO>().ReverseMap();
        }
    }
}
