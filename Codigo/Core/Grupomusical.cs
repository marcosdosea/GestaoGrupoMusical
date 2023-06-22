using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Grupomusical
    {
        public Grupomusical()
        {
            Ensaios = new HashSet<Ensaio>();
            Eventos = new HashSet<Evento>();
            Figurinos = new HashSet<Figurino>();
            Informativos = new HashSet<Informativo>();
            Instrumentomusicals = new HashSet<Instrumentomusical>();
            Materialestudos = new HashSet<Materialestudo>();
            Pessoas = new HashSet<Pessoa>();
            Receitafinanceiras = new HashSet<Receitafinanceira>();
        }

        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string? Cep { get; set; }
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string Estado { get; set; } = null!;
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Youtube { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Banco { get; set; }
        public string? Agencia { get; set; }
        public string? NumeroContaBanco { get; set; }
        public string? ChavePix { get; set; }
        public string? ChavePixtipo { get; set; }

        public virtual ICollection<Ensaio> Ensaios { get; set; }
        public virtual ICollection<Evento> Eventos { get; set; }
        public virtual ICollection<Figurino> Figurinos { get; set; }
        public virtual ICollection<Informativo> Informativos { get; set; }
        public virtual ICollection<Instrumentomusical> Instrumentomusicals { get; set; }
        public virtual ICollection<Materialestudo> Materialestudos { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
        public virtual ICollection<Receitafinanceira> Receitafinanceiras { get; set; }
    }
}
