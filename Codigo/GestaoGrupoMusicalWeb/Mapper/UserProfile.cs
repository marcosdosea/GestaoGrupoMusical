using AutoMapper;
using Core;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class UserProfile : Profile
    {   
        public UserProfile()
        {
            CreateMap<UserViewModel, Pessoa>().ReverseMap();
        }
        
    }
}
