using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class AdministradorGrupoMusicalProfile : Profile
    {
        public AdministradorGrupoMusicalProfile()
        {
            CreateMap<AdministradorGrupoMusicalViewModel, Pessoa>().ReverseMap();
        }

    }
}
