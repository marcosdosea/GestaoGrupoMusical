using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class GrupoMusicalContext : DbContext
{
    public GrupoMusicalContext()
    {
    }

    public GrupoMusicalContext(DbContextOptions<GrupoMusicalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Apresentacaotipoinstrumento> Apresentacaotipoinstrumentos { get; set; }

    public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

    public virtual DbSet<Aspnetroleclaim> Aspnetroleclaims { get; set; }

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    public virtual DbSet<Aspnetuserclaim> Aspnetuserclaims { get; set; }

    public virtual DbSet<Aspnetuserlogin> Aspnetuserlogins { get; set; }

    public virtual DbSet<Aspnetusertoken> Aspnetusertokens { get; set; }

    public virtual DbSet<Ensaio> Ensaios { get; set; }

    public virtual DbSet<Ensaiopessoa> Ensaiopessoas { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<Eventopessoa> Eventopessoas { get; set; }

    public virtual DbSet<Figurino> Figurinos { get; set; }

    public virtual DbSet<Figurinomanequim> Figurinomanequims { get; set; }

    public virtual DbSet<Grupomusical> Grupomusicals { get; set; }

    public virtual DbSet<Informativo> Informativos { get; set; }

    public virtual DbSet<Instrumentomusical> Instrumentomusicals { get; set; }

    public virtual DbSet<Manequim> Manequims { get; set; }

    public virtual DbSet<Materialestudo> Materialestudos { get; set; }

    public virtual DbSet<Movimentacaofigurino> Movimentacaofigurinos { get; set; }

    public virtual DbSet<Movimentacaoinstrumento> Movimentacaoinstrumentos { get; set; }

    public virtual DbSet<Papelgrupo> Papelgrupos { get; set; }

    public virtual DbSet<Pessoa> Pessoas { get; set; }

    public virtual DbSet<Receitafinanceira> Receitafinanceiras { get; set; }

    public virtual DbSet<Receitafinanceirapessoa> Receitafinanceirapessoas { get; set; }

    public virtual DbSet<Tipoinstrumento> Tipoinstrumentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=123456;database=grupomusical");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apresentacaotipoinstrumento>(entity =>
        {
            entity.HasKey(e => new { e.IdApresentacao, e.IdTipoInstrumento }).HasName("PRIMARY");

            entity.ToTable("apresentacaotipoinstrumento");

            entity.HasIndex(e => e.IdApresentacao, "fk_ApresentacaoTipoInstrumento_Apresentacao1_idx");

            entity.HasIndex(e => e.IdTipoInstrumento, "fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx");

            entity.Property(e => e.IdApresentacao).HasColumnName("idApresentacao");
            entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");
            entity.Property(e => e.QuantidadeConfirmada).HasColumnName("quantidadeConfirmada");
            entity.Property(e => e.QuantidadePlanejada).HasColumnName("quantidadePlanejada");

            entity.HasOne(d => d.IdApresentacaoNavigation).WithMany(p => p.Apresentacaotipoinstrumentos)
                .HasForeignKey(d => d.IdApresentacao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ApresentacaoTipoInstrumento_Apresentacao1");

            entity.HasOne(d => d.IdTipoInstrumentoNavigation).WithMany(p => p.Apresentacaotipoinstrumentos)
                .HasForeignKey(d => d.IdTipoInstrumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ApresentacaoTipoInstrumento_TipoInstrumento1");
        });

        modelBuilder.Entity<Aspnetrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetroles");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(767);
            entity.Property(e => e.ConcurrencyStamp).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<Aspnetroleclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetroleclaims");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.Property(e => e.ClaimType).HasColumnType("text");
            entity.Property(e => e.ClaimValue).HasColumnType("text");
            entity.Property(e => e.RoleId).HasMaxLength(767);

            entity.HasOne(d => d.Role).WithMany(p => p.Aspnetroleclaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
        });

        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetusers");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(767);
            entity.Property(e => e.ConcurrencyStamp).HasColumnType("text");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EmailConfirmed).HasColumnType("bit(1)");
            entity.Property(e => e.LockoutEnabled).HasColumnType("bit(1)");
            entity.Property(e => e.LockoutEnd).HasColumnType("timestamp");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasColumnType("text");
            entity.Property(e => e.PhoneNumber).HasColumnType("text");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnType("bit(1)");
            entity.Property(e => e.SecurityStamp).HasColumnType("text");
            entity.Property(e => e.TwoFactorEnabled).HasColumnType("bit(1)");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Aspnetuserrole",
                    r => r.HasOne<Aspnetrole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId"),
                    l => l.HasOne<Aspnetuser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PRIMARY");
                        j.ToTable("aspnetuserroles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        j.IndexerProperty<string>("UserId").HasMaxLength(767);
                        j.IndexerProperty<string>("RoleId").HasMaxLength(767);
                    });
        });

        modelBuilder.Entity<Aspnetuserclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetuserclaims");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.Property(e => e.ClaimType).HasColumnType("text");
            entity.Property(e => e.ClaimValue).HasColumnType("text");
            entity.Property(e => e.UserId).HasMaxLength(767);

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserclaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserClaims_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Aspnetuserlogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PRIMARY");

            entity.ToTable("aspnetuserlogins");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.ProviderDisplayName).HasColumnType("text");
            entity.Property(e => e.UserId).HasMaxLength(767);

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserlogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserLogins_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Aspnetusertoken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PRIMARY");

            entity.ToTable("aspnetusertokens");

            entity.Property(e => e.UserId).HasMaxLength(767);
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Value).HasColumnType("text");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetusertokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Ensaio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ensaio");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_Ensaio_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdColaboradorResponsavel, "fk_Ensaio_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHoraFim)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraFim");
            entity.Property(e => e.DataHoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraInicio");
            entity.Property(e => e.IdColaboradorResponsavel).HasColumnName("idColaboradorResponsavel");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.Local)
                .HasMaxLength(100)
                .HasColumnName("local");
            entity.Property(e => e.PresencaObrigatoria).HasColumnName("presencaObrigatoria");
            entity.Property(e => e.Repertorio)
                .HasMaxLength(1000)
                .HasColumnName("repertorio");
            entity.Property(e => e.Tipo)
                .HasDefaultValueSql("'FIXO'")
                .HasColumnType("enum('FIXO','EXTRA')")
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdColaboradorResponsavelNavigation).WithMany(p => p.Ensaios)
                .HasForeignKey(d => d.IdColaboradorResponsavel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ensaio_Pessoa1");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Ensaios)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ensaio_GrupoMusical1");
        });

        modelBuilder.Entity<Ensaiopessoa>(entity =>
        {
            entity.HasKey(e => new { e.IdPessoa, e.IdEnsaio }).HasName("PRIMARY");

            entity.ToTable("ensaiopessoa");

            entity.HasIndex(e => e.IdPapelGrupoPapelGrupo, "fk_EnsaioPessoa_PapelGrupo1_idx");

            entity.HasIndex(e => e.IdEnsaio, "fk_PessoaEnsaio_Ensaio1_idx");

            entity.HasIndex(e => e.IdPessoa, "fk_PessoaEnsaio_Pessoa1_idx");

            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.IdEnsaio).HasColumnName("idEnsaio");
            entity.Property(e => e.IdPapelGrupoPapelGrupo).HasColumnName("idPapelGrupoPapelGrupo");
            entity.Property(e => e.JustificativaAceita).HasColumnName("justificativaAceita");
            entity.Property(e => e.JustificativaFalta)
                .HasMaxLength(200)
                .HasColumnName("justificativaFalta");
            entity.Property(e => e.Presente).HasColumnName("presente");

            entity.HasOne(d => d.IdEnsaioNavigation).WithMany(p => p.Ensaiopessoas)
                .HasForeignKey(d => d.IdEnsaio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PessoaEnsaio_Ensaio1");

            entity.HasOne(d => d.IdPapelGrupoPapelGrupoNavigation).WithMany(p => p.Ensaiopessoas)
                .HasForeignKey(d => d.IdPapelGrupoPapelGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EnsaioPessoa_PapelGrupo1");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Ensaiopessoas)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PessoaEnsaio_Pessoa1");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("evento");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_Ensaio_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdColaboradorResponsavel, "fk_Ensaio_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataHoraFim)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraFim");
            entity.Property(e => e.DataHoraInicio)
                .HasColumnType("datetime")
                .HasColumnName("dataHoraInicio");
            entity.Property(e => e.IdColaboradorResponsavel).HasColumnName("idColaboradorResponsavel");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.Local)
                .HasMaxLength(100)
                .HasColumnName("local");
            entity.Property(e => e.Repertorio)
                .HasMaxLength(1000)
                .HasColumnName("repertorio");

            entity.HasOne(d => d.IdColaboradorResponsavelNavigation).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.IdColaboradorResponsavel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ensaio_Pessoa10");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ensaio_GrupoMusical10");
        });

        modelBuilder.Entity<Eventopessoa>(entity =>
        {
            entity.HasKey(e => new { e.IdEvento, e.IdPessoa }).HasName("PRIMARY");

            entity.ToTable("eventopessoa");

            entity.HasIndex(e => e.IdEvento, "fk_ApresentacaoPessoa_Apresentacao1_idx");

            entity.HasIndex(e => e.IdPessoa, "fk_ApresentacaoPessoa_Pessoa1_idx");

            entity.HasIndex(e => e.IdTipoInstrumento, "fk_ApresentacaoPessoa_TipoInstrumento1_idx");

            entity.HasIndex(e => e.IdPapelGrupoPapelGrupo, "fk_EventoPessoa_PapelGrupo1_idx");

            entity.Property(e => e.IdEvento).HasColumnName("idEvento");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.IdPapelGrupoPapelGrupo).HasColumnName("idPapelGrupoPapelGrupo");
            entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");
            entity.Property(e => e.JustificativaAceita).HasColumnName("justificativaAceita");
            entity.Property(e => e.JustificativaFalta)
                .HasMaxLength(200)
                .HasColumnName("justificativaFalta");
            entity.Property(e => e.Presente).HasColumnName("presente");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'INSCRITO'")
                .HasColumnType("enum('INSCRITO','DEFERIDO','INDEFERIDO')")
                .HasColumnName("status");

            entity.HasOne(d => d.IdEventoNavigation).WithMany(p => p.Eventopessoas)
                .HasForeignKey(d => d.IdEvento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ApresentacaoPessoa_Apresentacao1");

            entity.HasOne(d => d.IdPapelGrupoPapelGrupoNavigation).WithMany(p => p.Eventopessoas)
                .HasForeignKey(d => d.IdPapelGrupoPapelGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EventoPessoa_PapelGrupo1");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Eventopessoas)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ApresentacaoPessoa_Pessoa1");

            entity.HasOne(d => d.IdTipoInstrumentoNavigation).WithMany(p => p.Eventopessoas)
                .HasForeignKey(d => d.IdTipoInstrumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ApresentacaoPessoa_TipoInstrumento1");
        });

        modelBuilder.Entity<Figurino>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("figurino");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_Figurino_GrupoMusical1_idx");

            entity.HasIndex(e => e.Id, "id_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("date")
                .HasColumnName("data");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Figurinos)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Figurino_GrupoMusical1");

            entity.HasMany(d => d.IdApresentacaos).WithMany(p => p.IdFigurinos)
                .UsingEntity<Dictionary<string, object>>(
                    "Figurinoapresentacao",
                    r => r.HasOne<Evento>().WithMany()
                        .HasForeignKey("IdApresentacao")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_FigurinoApresentacao_Apresentacao1"),
                    l => l.HasOne<Figurino>().WithMany()
                        .HasForeignKey("IdFigurino")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_FigurinoApresentacao_Figurino1"),
                    j =>
                    {
                        j.HasKey("IdFigurino", "IdApresentacao").HasName("PRIMARY");
                        j.ToTable("figurinoapresentacao");
                        j.HasIndex(new[] { "IdApresentacao" }, "fk_FigurinoApresentacao_Apresentacao1_idx");
                        j.HasIndex(new[] { "IdFigurino" }, "fk_FigurinoApresentacao_Figurino1_idx");
                        j.IndexerProperty<int>("IdFigurino").HasColumnName("idFigurino");
                        j.IndexerProperty<int>("IdApresentacao").HasColumnName("idApresentacao");
                    });

            entity.HasMany(d => d.IdEnsaios).WithMany(p => p.IdFigurinos)
                .UsingEntity<Dictionary<string, object>>(
                    "Figurinoensaio",
                    r => r.HasOne<Ensaio>().WithMany()
                        .HasForeignKey("IdEnsaio")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_FigurinoEnsaio_Ensaio1"),
                    l => l.HasOne<Figurino>().WithMany()
                        .HasForeignKey("IdFigurino")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_FigurinoEnsaio_Figurino1"),
                    j =>
                    {
                        j.HasKey("IdFigurino", "IdEnsaio").HasName("PRIMARY");
                        j.ToTable("figurinoensaio");
                        j.HasIndex(new[] { "IdEnsaio" }, "fk_FigurinoEnsaio_Ensaio1_idx");
                        j.HasIndex(new[] { "IdFigurino" }, "fk_FigurinoEnsaio_Figurino1_idx");
                        j.IndexerProperty<int>("IdFigurino").HasColumnName("idFigurino");
                        j.IndexerProperty<int>("IdEnsaio").HasColumnName("idEnsaio");
                    });
        });

        modelBuilder.Entity<Figurinomanequim>(entity =>
        {
            entity.HasKey(e => new { e.IdFigurino, e.IdManequim }).HasName("PRIMARY");

            entity.ToTable("figurinomanequim");

            entity.HasIndex(e => e.IdFigurino, "fk_FigurinoManequim_Figurino1_idx");

            entity.HasIndex(e => e.IdManequim, "fk_FigurinoManequim_Manequim1_idx");

            entity.Property(e => e.IdFigurino).HasColumnName("idFigurino");
            entity.Property(e => e.IdManequim).HasColumnName("idManequim");
            entity.Property(e => e.QuantidadeDescartada).HasColumnName("quantidadeDescartada");
            entity.Property(e => e.QuantidadeDisponivel).HasColumnName("quantidadeDisponivel");
            entity.Property(e => e.QuantidadeEntregue).HasColumnName("quantidadeEntregue");

            entity.HasOne(d => d.IdFigurinoNavigation).WithMany(p => p.Figurinomanequims)
                .HasForeignKey(d => d.IdFigurino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FigurinoManequim_Figurino1");

            entity.HasOne(d => d.IdManequimNavigation).WithMany(p => p.Figurinomanequims)
                .HasForeignKey(d => d.IdManequim)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FigurinoManequim_Manequim1");
        });

        modelBuilder.Entity<Grupomusical>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("grupomusical");

            entity.HasIndex(e => e.Cnpj, "cnpj_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agencia)
                .HasMaxLength(15)
                .HasColumnName("agencia");
            entity.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("bairro");
            entity.Property(e => e.Banco)
                .HasMaxLength(100)
                .HasColumnName("banco");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.ChavePix)
                .HasMaxLength(100)
                .HasColumnName("chavePIX");
            entity.Property(e => e.ChavePixtipo)
                .HasMaxLength(15)
                .HasColumnName("chavePIXTipo");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("cidade");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(14)
                .HasColumnName("cnpj");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .HasDefaultValueSql("'SE'")
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.Facebook)
                .HasMaxLength(100)
                .HasColumnName("facebook");
            entity.Property(e => e.Instagram)
                .HasMaxLength(100)
                .HasColumnName("instagram");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.NumeroContaBanco)
                .HasMaxLength(15)
                .HasColumnName("numeroContaBanco");
            entity.Property(e => e.Pais)
                .HasMaxLength(50)
                .HasColumnName("pais");
            entity.Property(e => e.RazaoSocial)
                .HasMaxLength(100)
                .HasColumnName("razaoSocial");
            entity.Property(e => e.Rua)
                .HasMaxLength(100)
                .HasColumnName("rua");
            entity.Property(e => e.Telefone1)
                .HasMaxLength(20)
                .HasColumnName("telefone1");
            entity.Property(e => e.Telefone2)
                .HasMaxLength(20)
                .HasColumnName("telefone2");
            entity.Property(e => e.Youtube)
                .HasMaxLength(100)
                .HasColumnName("youtube");
        });

        modelBuilder.Entity<Informativo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("informativo");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_GrupoMusicalPessoa_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdPessoa, "fk_GrupoMusicalPessoa_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("date")
                .HasColumnName("data");
            entity.Property(e => e.EntregarAssociadosAtivos).HasColumnName("entregarAssociadosAtivos");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.Mensagem)
                .HasMaxLength(2000)
                .HasColumnName("mensagem");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Informativos)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_GrupoMusicalPessoa_GrupoMusical1");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Informativos)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_GrupoMusicalPessoa_Pessoa1");
        });

        modelBuilder.Entity<Instrumentomusical>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("instrumentomusical");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_InstrumentoMusical_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdTipoInstrumento, "fk_InstrumentoMusical_TipoInstrumento1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataAquisicao)
                .HasColumnType("date")
                .HasColumnName("dataAquisicao");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");
            entity.Property(e => e.Patrimonio)
                .HasMaxLength(20)
                .HasColumnName("patrimonio");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'DISPONIVEL'")
                .HasColumnType("enum('DISPONIVEL','EMPRESTADO','DANIFICADO')")
                .HasColumnName("status");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Instrumentomusicals)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_InstrumentoMusical_GrupoMusical1");

            entity.HasOne(d => d.IdTipoInstrumentoNavigation).WithMany(p => p.Instrumentomusicals)
                .HasForeignKey(d => d.IdTipoInstrumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_InstrumentoMusical_TipoInstrumento1");
        });

        modelBuilder.Entity<Manequim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("manequim");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(100)
                .HasColumnName("descricao");
            entity.Property(e => e.Tamanho)
                .HasMaxLength(2)
                .HasColumnName("tamanho");
        });

        modelBuilder.Entity<Materialestudo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("materialestudo");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_MaterialEstudo_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdColaborador, "fk_MaterialEstudo_Pessoa1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.IdColaborador).HasColumnName("idColaborador");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.Link)
                .HasMaxLength(500)
                .HasColumnName("link");
            entity.Property(e => e.Nome)
                .HasMaxLength(200)
                .HasColumnName("nome");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.Materialestudos)
                .HasForeignKey(d => d.IdColaborador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MaterialEstudo_Pessoa1");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Materialestudos)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MaterialEstudo_GrupoMusical1");
        });

        modelBuilder.Entity<Movimentacaofigurino>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movimentacaofigurino");

            entity.HasIndex(e => e.IdFigurino, "fk_EntregarFigurino_Figurino1_idx");

            entity.HasIndex(e => e.IdManequim, "fk_EntregarFigurino_Manequim1_idx");

            entity.HasIndex(e => e.IdAssociado, "fk_EntregarFigurino_Pessoa1_idx");

            entity.HasIndex(e => e.IdColaborador, "fk_EntregarFigurino_Pessoa2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmacaoRecebimento).HasColumnName("confirmacaoRecebimento");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.IdAssociado).HasColumnName("idAssociado");
            entity.Property(e => e.IdColaborador).HasColumnName("idColaborador");
            entity.Property(e => e.IdFigurino).HasColumnName("idFigurino");
            entity.Property(e => e.IdManequim).HasColumnName("idManequim");
            entity.Property(e => e.Quantidade)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantidade");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'DISPONIVEL'")
                .HasColumnType("enum('DISPONIVEL','ENTREGUE','RECEBIDO','DANIFICADO','DEVOLVIDO')")
                .HasColumnName("status");

            entity.HasOne(d => d.IdAssociadoNavigation).WithMany(p => p.MovimentacaofigurinoIdAssociadoNavigations)
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EntregarFigurino_Pessoa1");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.MovimentacaofigurinoIdColaboradorNavigations)
                .HasForeignKey(d => d.IdColaborador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EntregarFigurino_Pessoa2");

            entity.HasOne(d => d.IdFigurinoNavigation).WithMany(p => p.Movimentacaofigurinos)
                .HasForeignKey(d => d.IdFigurino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EntregarFigurino_Figurino1");

            entity.HasOne(d => d.IdManequimNavigation).WithMany(p => p.Movimentacaofigurinos)
                .HasForeignKey(d => d.IdManequim)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EntregarFigurino_Manequim1");
        });

        modelBuilder.Entity<Movimentacaoinstrumento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("movimentacaoinstrumento");

            entity.HasIndex(e => e.IdInstrumentoMusical, "fk_DevolverInstrumento_InstrumentoMusical1_idx");

            entity.HasIndex(e => e.IdAssociado, "fk_DevolverInstrumento_Pessoa1_idx");

            entity.HasIndex(e => e.IdColaborador, "fk_DevolverInstrumento_Pessoa2_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConfirmacaoAssociado).HasColumnName("confirmacaoAssociado");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.IdAssociado).HasColumnName("idAssociado");
            entity.Property(e => e.IdColaborador).HasColumnName("idColaborador");
            entity.Property(e => e.IdInstrumentoMusical).HasColumnName("idInstrumentoMusical");
            entity.Property(e => e.TipoMovimento)
                .HasDefaultValueSql("'EMPRESTIMO'")
                .HasColumnType("enum('EMPRESTIMO','DEVOLUCAO')")
                .HasColumnName("tipoMovimento");

            entity.HasOne(d => d.IdAssociadoNavigation).WithMany(p => p.MovimentacaoinstrumentoIdAssociadoNavigations)
                .HasForeignKey(d => d.IdAssociado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DevolverInstrumento_Pessoa1");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.MovimentacaoinstrumentoIdColaboradorNavigations)
                .HasForeignKey(d => d.IdColaborador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DevolverInstrumento_Pessoa2");

            entity.HasOne(d => d.IdInstrumentoMusicalNavigation).WithMany(p => p.Movimentacaoinstrumentos)
                .HasForeignKey(d => d.IdInstrumentoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DevolverInstrumento_InstrumentoMusical1");
        });

        modelBuilder.Entity<Papelgrupo>(entity =>
        {
            entity.HasKey(e => e.IdPapelGrupo).HasName("PRIMARY");

            entity.ToTable("papelgrupo");

            entity.Property(e => e.IdPapelGrupo).HasColumnName("idPapelGrupo");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
        });

        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pessoa");

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.HasIndex(e => e.IdGrupoMusical, "fk_Pessoa_GrupoMusical1_idx");

            entity.HasIndex(e => e.IdManequim, "fk_Pessoa_Manequim1_idx");

            entity.HasIndex(e => e.IdPapelGrupo, "fk_Pessoa_PapelGrupoMusical1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ativo).HasColumnName("ativo");
            entity.Property(e => e.Bairro)
                .HasMaxLength(100)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .HasColumnName("cidade");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .HasColumnName("cpf");
            entity.Property(e => e.DataEntrada)
                .HasColumnType("date")
                .HasColumnName("dataEntrada");
            entity.Property(e => e.DataNascimento)
                .HasColumnType("date")
                .HasColumnName("dataNascimento");
            entity.Property(e => e.DataSaida)
                .HasColumnType("date")
                .HasColumnName("dataSaida");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .HasDefaultValueSql("'SE'")
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.IdManequim).HasColumnName("idManequim");
            entity.Property(e => e.IdPapelGrupo).HasColumnName("idPapelGrupo");
            entity.Property(e => e.IsentoPagamento).HasColumnName("isentoPagamento");
            entity.Property(e => e.MotivoSaida)
                .HasMaxLength(100)
                .HasColumnName("motivoSaida");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Rua)
                .HasMaxLength(100)
                .HasColumnName("rua");
            entity.Property(e => e.Sexo)
                .HasDefaultValueSql("'F'")
                .HasColumnType("enum('M','F')")
                .HasColumnName("sexo");
            entity.Property(e => e.Telefone1)
                .HasMaxLength(20)
                .HasColumnName("telefone1");
            entity.Property(e => e.Telefone2)
                .HasMaxLength(20)
                .HasColumnName("telefone2");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pessoa_GrupoMusical1");

            entity.HasOne(d => d.IdManequimNavigation).WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdManequim)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pessoa_Manequim1");

            entity.HasOne(d => d.IdPapelGrupoNavigation).WithMany(p => p.Pessoas)
                .HasForeignKey(d => d.IdPapelGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pessoa_PapelGrupoMusical1");

            entity.HasMany(d => d.IdTipoInstrumentos).WithMany(p => p.IdPessoas)
                .UsingEntity<Dictionary<string, object>>(
                    "Pessoatipoinstrumento",
                    r => r.HasOne<Tipoinstrumento>().WithMany()
                        .HasForeignKey("IdTipoInstrumento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_Pessoa_has_TipoInstrumento_TipoInstrumento1"),
                    l => l.HasOne<Pessoa>().WithMany()
                        .HasForeignKey("IdPessoa")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_Pessoa_has_TipoInstrumento_Pessoa"),
                    j =>
                    {
                        j.HasKey("IdPessoa", "IdTipoInstrumento").HasName("PRIMARY");
                        j.ToTable("pessoatipoinstrumento");
                        j.HasIndex(new[] { "IdPessoa" }, "fk_Pessoa_has_TipoInstrumento_Pessoa_idx");
                        j.HasIndex(new[] { "IdTipoInstrumento" }, "fk_Pessoa_has_TipoInstrumento_TipoInstrumento1_idx");
                        j.IndexerProperty<int>("IdPessoa").HasColumnName("idPessoa");
                        j.IndexerProperty<int>("IdTipoInstrumento").HasColumnName("idTipoInstrumento");
                    });
        });

        modelBuilder.Entity<Receitafinanceira>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("receitafinanceira");

            entity.HasIndex(e => e.IdGrupoMusical, "fk_ReceitaFinanceira_GrupoMusical1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataFim)
                .HasColumnType("date")
                .HasColumnName("dataFim");
            entity.Property(e => e.DataInicio)
                .HasColumnType("date")
                .HasColumnName("dataInicio");
            entity.Property(e => e.Descricao)
                .HasMaxLength(100)
                .HasColumnName("descricao");
            entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");
            entity.Property(e => e.Valor)
                .HasPrecision(10)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdGrupoMusicalNavigation).WithMany(p => p.Receitafinanceiras)
                .HasForeignKey(d => d.IdGrupoMusical)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceitaFinanceira_GrupoMusical1");
        });

        modelBuilder.Entity<Receitafinanceirapessoa>(entity =>
        {
            entity.HasKey(e => new { e.IdReceitaFinanceira, e.IdPessoa }).HasName("PRIMARY");

            entity.ToTable("receitafinanceirapessoa");

            entity.HasIndex(e => e.IdPessoa, "fk_ReceitaFinanceiraPessoa_Pessoa1_idx");

            entity.HasIndex(e => e.IdReceitaFinanceira, "fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1_idx");

            entity.Property(e => e.IdReceitaFinanceira).HasColumnName("idReceitaFinanceira");
            entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");
            entity.Property(e => e.DataPagamento)
                .HasColumnType("datetime")
                .HasColumnName("dataPagamento");
            entity.Property(e => e.Observacoes)
                .HasMaxLength(200)
                .HasColumnName("observacoes");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'ABERTO'")
                .HasColumnType("enum('ABERTO','ENVIADO','PAGO','ISENTO')")
                .HasColumnName("status");
            entity.Property(e => e.Valor)
                .HasPrecision(10)
                .HasColumnName("valor");
            entity.Property(e => e.ValorPago)
                .HasPrecision(10)
                .HasColumnName("valorPago");

            entity.HasOne(d => d.IdPessoaNavigation).WithMany(p => p.Receitafinanceirapessoas)
                .HasForeignKey(d => d.IdPessoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceitaFinanceiraPessoa_Pessoa1");

            entity.HasOne(d => d.IdReceitaFinanceiraNavigation).WithMany(p => p.Receitafinanceirapessoas)
                .HasForeignKey(d => d.IdReceitaFinanceira)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1");
        });

        modelBuilder.Entity<Tipoinstrumento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tipoinstrumento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
