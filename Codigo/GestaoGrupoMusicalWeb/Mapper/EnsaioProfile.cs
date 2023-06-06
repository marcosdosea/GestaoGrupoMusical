using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EnsaioProfile : Profile
    {
        public EnsaioProfile()
        {
            CreateMap<EnsaioViewModel, Ensaio>().ReverseMap();
        }
    }
}
