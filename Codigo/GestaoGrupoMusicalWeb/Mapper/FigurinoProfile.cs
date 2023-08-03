using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class FigurinoProfile : Profile
    {
        public FigurinoProfile()
        {
            CreateMap<Figurino, FigurinoViewModel>().ReverseMap();
        }
    }
}
