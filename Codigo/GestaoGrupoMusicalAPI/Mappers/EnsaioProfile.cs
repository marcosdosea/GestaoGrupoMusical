using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mapper
{
    public class EnsaioProfile : Profile
    {
        public EnsaioProfile()
        {   
            CreateMap<Ensaio, EnsaioDTO>().ReverseMap();
            CreateMap<Ensaio, EnsaioDetailsDTO>().ReverseMap();
        }
    }
}

