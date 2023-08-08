﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EstoqueDTO
    {
        public string? Nome { get; set; }
        public DateTime? Data { get; set; }
        public string? Tamanho { get; set; }
        public int Disponivel { get; set; } 
        public int Entregues { get; set; }
    }
}
