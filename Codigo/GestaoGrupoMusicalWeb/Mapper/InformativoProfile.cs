using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class InformativoProfile : Profile
    {

        public InformativoProfile()
        {
            CreateMap<InformativoViewModel, Informativo>().ReverseMap();
        }
    }
}

