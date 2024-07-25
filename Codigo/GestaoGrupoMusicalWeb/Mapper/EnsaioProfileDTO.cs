using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EnsaioProfileDTO : Profile
    {
        public EnsaioProfileDTO()
        {
            CreateMap<EnsaioViewModel, Ensaio>().ReverseMap();
        }
    }
}
