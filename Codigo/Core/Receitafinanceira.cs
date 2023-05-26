using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Receitafinanceira
    {
        public Receitafinanceira()
        {
            Receitafinanceirapessoas = new HashSet<Receitafinanceirapessoa>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; } = null!;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public decimal Valor { get; set; }
        public int IdGrupoMusical { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual ICollection<Receitafinanceirapessoa> Receitafinanceirapessoas { get; set; }
    }
}
