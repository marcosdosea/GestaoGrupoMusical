using System;
using System.Collections.Generic;

namespace Core
{
    public partial class Pessoa
    {
        public Pessoa()
        {
            ApresentacaoIdColaboradorResponsavelNavigations = new HashSet<Apresentacao>();
            ApresentacaoIdRegenteNavigations = new HashSet<Apresentacao>();
            Apresentacaopessoas = new HashSet<Apresentacaopessoa>();
            EnsaioIdColaboradorResponsavelNavigations = new HashSet<Ensaio>();
            EnsaioIdRegenteNavigations = new HashSet<Ensaio>();
            Ensaiopessoas = new HashSet<Ensaiopessoa>();
            EntregarfigurinoIdAssociadoNavigations = new HashSet<Entregarfigurino>();
            EntregarfigurinoIdColaboradorNavigations = new HashSet<Entregarfigurino>();
            MovimentacaoinstrumentoIdAssociadoNavigations = new HashSet<Movimentacaoinstrumento>();
            MovimentacaoinstrumentoIdColaboradorNavigations = new HashSet<Movimentacaoinstrumento>();
            IdTipoInstrumentos = new HashSet<Tipoinstrumento>();
        }

        public int Id { get; set; }
        public string Cpf { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string Sexo { get; set; } = null!;
        public string? Cep { get; set; }
        public string? Rua { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string Estado { get; set; } = null!;
        public DateTime? DataNascimento { get; set; }
        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Email { get; set; }
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
        public virtual ICollection<Apresentacao> ApresentacaoIdColaboradorResponsavelNavigations { get; set; }
        public virtual ICollection<Apresentacao> ApresentacaoIdRegenteNavigations { get; set; }
        public virtual ICollection<Apresentacaopessoa> Apresentacaopessoas { get; set; }
        public virtual ICollection<Ensaio> EnsaioIdColaboradorResponsavelNavigations { get; set; }
        public virtual ICollection<Ensaio> EnsaioIdRegenteNavigations { get; set; }
        public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; }
        public virtual ICollection<Entregarfigurino> EntregarfigurinoIdAssociadoNavigations { get; set; }
        public virtual ICollection<Entregarfigurino> EntregarfigurinoIdColaboradorNavigations { get; set; }
        public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdAssociadoNavigations { get; set; }
        public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdColaboradorNavigations { get; set; }

        public virtual ICollection<Tipoinstrumento> IdTipoInstrumentos { get; set; }
    }
}
