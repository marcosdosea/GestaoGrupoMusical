﻿using AutoMapper;
using Core;
using Core.DTO;
using GestaoGrupoMusicalWeb.Models;

namespace GestaoGrupoMusicalWeb.Mapper
{
    public class EventoProfile : Profile
    {
        public EventoProfile()
        {
            CreateMap<EventoViewModel, Evento>().ReverseMap();
            CreateMap<EventoCreateViewlModel, Evento>().ReverseMap();
        }
    }
}
