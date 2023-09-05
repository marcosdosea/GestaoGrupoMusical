﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class EstoqueDTO
    {
        public int IdManequim { get; set; }
        public int IdFigurino { get; set; }
        public string? Tamanho { get; set; }
        public int Disponivel { get; set; }
        public int Entregues { get; set; }

        public int Danificados { get; set; }

        public string TamanhoEstoque
        {
            get { return Tamanho + " - " + Disponivel + " disponíveis"; }
        }
    }
}
