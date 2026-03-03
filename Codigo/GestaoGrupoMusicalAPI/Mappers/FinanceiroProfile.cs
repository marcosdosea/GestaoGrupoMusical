using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalAPI.Models;

namespace GestaoGrupoMusicalAPI.Mapper
{
    public class FinanceiroProfile : Profile
    {
        public FinanceiroProfile()
        {
            // Mapeamento para a camada de serviço (DTO)
            CreateMap<FinanceiroViewModel, FinanceiroCreateDTO>();

            // Mapeamento para a Entidade do Banco (usado no Edit POST)
            CreateMap<FinanceiroViewModel, Receitafinanceira>();

            // Mapeamento da Entidade do Banco para o ViewModel (usado no Edit GET)
            CreateMap<Receitafinanceira, FinanceiroViewModel>();
        }
    }
}