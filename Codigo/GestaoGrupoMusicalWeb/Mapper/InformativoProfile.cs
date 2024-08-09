using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
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

