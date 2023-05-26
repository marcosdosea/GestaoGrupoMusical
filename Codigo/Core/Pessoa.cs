using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Pessoa
    {
        public Pessoa()
        {
            EnsaioIdColaboradorResponsavelNavigations = new HashSet<Ensaio>();
            EnsaioIdRegenteNavigations = new HashSet<Ensaio>();
            Ensaiopessoas = new HashSet<Ensaiopessoa>();
            EventoIdColaboradorResponsavelNavigations = new HashSet<Evento>();
            EventoIdRegenteNavigations = new HashSet<Evento>();
            Eventopessoas = new HashSet<Eventopessoa>();
            Materialestudos = new HashSet<Materialestudo>();
            MovimentacaofigurinoIdAssociadoNavigations = new HashSet<Movimentacaofigurino>();
            MovimentacaofigurinoIdColaboradorNavigations = new HashSet<Movimentacaofigurino>();
            MovimentacaoinstrumentoIdAssociadoNavigations = new HashSet<Movimentacaoinstrumento>();
            MovimentacaoinstrumentoIdColaboradorNavigations = new HashSet<Movimentacaoinstrumento>();
            Receitafinanceirapessoas = new HashSet<Receitafinanceirapessoa>();
            IdTipoInstrumentos = new HashSet<Tipoinstrumento>();
        }

        public int Id { get; set; }
        public string Cpf { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string Sexo { get; set; } = null!;
        public string Cep { get; set; } = null!;
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? DataNascimento { get; set; }
        public string Telefone1 { get; set; } = null!;
        public string? Telefone2 { get; set; }
        public string Email { get; set; } = null!;
        public DateTime? DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public string? MotivoSaida { get; set; }
        public sbyte Ativo { get; set; }
        public sbyte IsentoPagamento { get; set; }
        public int IdGrupoMusical { get; set; }
        public int IdPapelGrupo { get; set; }
        public int IdManequim { get; set; }

        public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;
        public virtual Manequim IdManequimNavigation { get; set; } = null!;
        public virtual Papelgrupo IdPapelGrupoNavigation { get; set; } = null!;
        public virtual ICollection<Ensaio> EnsaioIdColaboradorResponsavelNavigations { get; set; }
        public virtual ICollection<Ensaio> EnsaioIdRegenteNavigations { get; set; }
        public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; }
        public virtual ICollection<Evento> EventoIdColaboradorResponsavelNavigations { get; set; }
        public virtual ICollection<Evento> EventoIdRegenteNavigations { get; set; }
        public virtual ICollection<Eventopessoa> Eventopessoas { get; set; }
        public virtual ICollection<Materialestudo> Materialestudos { get; set; }
        public virtual ICollection<Movimentacaofigurino> MovimentacaofigurinoIdAssociadoNavigations { get; set; }
        public virtual ICollection<Movimentacaofigurino> MovimentacaofigurinoIdColaboradorNavigations { get; set; }
        public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdAssociadoNavigations { get; set; }
        public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdColaboradorNavigations { get; set; }
        public virtual ICollection<Receitafinanceirapessoa> Receitafinanceirapessoas { get; set; }

        public virtual ICollection<Tipoinstrumento> IdTipoInstrumentos { get; set; }
    }
}
