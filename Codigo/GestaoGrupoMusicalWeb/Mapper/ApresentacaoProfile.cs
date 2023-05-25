using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class ApresentacaoProfile : Profile
    {
        public ApresentacaoProfile()
        {
            CreateMap<ApresentacaoViewModel, Apresentacao>().ReverseMap();
        }
    }
}
