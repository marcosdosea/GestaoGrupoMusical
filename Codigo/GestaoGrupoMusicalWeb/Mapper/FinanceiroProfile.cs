using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class FinanceiroProfile : Profile
    {
        public FinanceiroProfile()
        {
            // Mapeamento para a camada de serviço (DTO)
            CreateMap<FinanceiroCreateViewModel, FinanceiroCreateDTO>();

            // Mapeamento para a Entidade do Banco (usado no Edit POST)
            CreateMap<FinanceiroCreateViewModel, Receitafinanceira>();

            // Mapeamento da Entidade do Banco para o ViewModel (usado no Edit GET)
            CreateMap<Receitafinanceira, FinanceiroCreateViewModel>();
        }
    }
}