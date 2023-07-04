using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class InformativoDTO
    {
        public int IdGrupoMusical { get; set; }
        public int IdPessoa { get; set; }
        public string Mensagem { get; set; } = null!;
        public DateTime Data { get; set; }
    }
}
