using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EnsaioProfile : Profile
    {
        public EnsaioProfile()
        {
            CreateMap<EnsaioViewModel, Ensaio>().ReverseMap();
            CreateMap<FrequenciaEnsaioViewModel, FrequenciaEnsaioDTO>().ReverseMap();
        }
    }
}
