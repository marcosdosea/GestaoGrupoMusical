using AutoMapper;
using Core;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mapper
{
    public class PessoaProfile : Profile
    {   
        public PessoaProfile()
        {
            CreateMap<PessoaViewModel, Pessoa>().ReverseMap();
        }
        
    }
}
