using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mapper
{
    public class InformativoProfile : Profile
    {

        public InformativoProfile()
        {
            CreateMap<InformativoViewModel, Informativo>().ReverseMap();
            CreateMap<InformativoIndexDTO, Informativo>().ReverseMap();
        }
    }
}

