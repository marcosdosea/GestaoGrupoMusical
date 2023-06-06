using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class InstrumentoMusicalProfile : Profile
    {
        public InstrumentoMusicalProfile()
        {
            CreateMap<InstrumentoMusicalViewModel, Instrumentomusical>().ReverseMap();
        }
    }
}
