using AutoMapper;
using Core;
using Core.DTO;

namespace GestaoGrupoMusicalWeb.Models
{
    public class EstoqueDTOProfile : Profile
    {
        public EstoqueDTOProfile()
        {
            CreateMap<EstoqueViewModel, EstoqueDTO>().ReverseMap();
        }
    }
}
