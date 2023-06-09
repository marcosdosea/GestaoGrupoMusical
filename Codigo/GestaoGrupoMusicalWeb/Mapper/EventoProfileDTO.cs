﻿using AutoMapper;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EventoProfileDTO : Profile
    {
        public EventoProfileDTO()
        {
            CreateMap<EventoViewModelDTO, EventoDTO>().ReverseMap();
        }
    }
}
