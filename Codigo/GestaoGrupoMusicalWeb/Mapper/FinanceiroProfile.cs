using AutoMapper;
using Core.DTO;
using Core;
using GestaoGrupoMusicalWeb.Models;
using System.Runtime.ConstrainedExecution;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class FinanceiroProfile : Profile
    {
        public FinanceiroProfile()
        {
            CreateMap<FinanceiroCreateDTO, FinanceiroCreateViewModel>().ReverseMap();
            CreateMap<FinanceiroCreateDTO, Receitafinanceira>().ReverseMap();
        }
    }
}