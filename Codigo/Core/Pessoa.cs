using System;
using System.Collections.Generic;

namespace Core;

public partial class Pessoa
{
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

    public virtual ICollection<Ensaiopessoa> Ensaiopessoas { get; set; } = new List<Ensaiopessoa>();

    public virtual ICollection<Ensaio> Ensaios { get; set; } = new List<Ensaio>();

    public virtual ICollection<Eventopessoa> Eventopessoas { get; set; } = new List<Eventopessoa>();

    public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();

    public virtual Grupomusical IdGrupoMusicalNavigation { get; set; } = null!;

    public virtual Manequim IdManequimNavigation { get; set; } = null!;

    public virtual Papelgrupo IdPapelGrupoNavigation { get; set; } = null!;

    public virtual ICollection<Informativo> Informativos { get; set; } = new List<Informativo>();

    public virtual ICollection<Materialestudo> Materialestudos { get; set; } = new List<Materialestudo>();

    public virtual ICollection<Movimentacaofigurino> MovimentacaofigurinoIdAssociadoNavigations { get; set; } = new List<Movimentacaofigurino>();

    public virtual ICollection<Movimentacaofigurino> MovimentacaofigurinoIdColaboradorNavigations { get; set; } = new List<Movimentacaofigurino>();

    public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdAssociadoNavigations { get; set; } = new List<Movimentacaoinstrumento>();

    public virtual ICollection<Movimentacaoinstrumento> MovimentacaoinstrumentoIdColaboradorNavigations { get; set; } = new List<Movimentacaoinstrumento>();

    public virtual ICollection<Receitafinanceirapessoa> Receitafinanceirapessoas { get; set; } = new List<Receitafinanceirapessoa>();

    public virtual ICollection<Tipoinstrumento> IdTipoInstrumentos { get; set; } = new List<Tipoinstrumento>();
}
