﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class InstrumentoPlanejadoEventoDTO
    {
        public int Id { get; set; }
        public int IdInstrumento { get; set; }
        public int Planejados { get; set; }
        public int Solicitdos { get; set; }
        public int Confirmados { get; set; }
        public string GetNomeInstrumento { get; set; }
    }
}