using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class AssociadoDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public sbyte Ativo { get; set; }
    }

    public class ColaboradoresDTO
    {
        public int Id { get; set; }

        public string? Cpf { get; set; }

        public string? Nome { get; set; }

        public string? Data { get; set; }

        public string? Papel { get; set; }
    }
}
