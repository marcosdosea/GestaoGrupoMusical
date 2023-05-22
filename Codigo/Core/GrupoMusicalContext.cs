﻿using Microsoft.EntityFrameworkCore;

namespace Core
{
    public partial class GrupoMusicalContext : DbContext
    {
        public GrupoMusicalContext()
        {
        }

        public GrupoMusicalContext(DbContextOptions<GrupoMusicalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Apresentacao> Apresentacaos { get; set; } = null!;
        public virtual DbSet<Apresentacaopessoa> Apresentacaopessoas { get; set; } = null!;
        public virtual DbSet<Apresentacaotipoinstrumento> Apresentacaotipoinstrumentos { get; set; } = null!;
        public virtual DbSet<Ensaio> Ensaios { get; set; } = null!;
        public virtual DbSet<Ensaiopessoa> Ensaiopessoas { get; set; } = null!;
        public virtual DbSet<Entregarfigurino> Entregarfigurinos { get; set; } = null!;
        public virtual DbSet<Figurino> Figurinos { get; set; } = null!;
        public virtual DbSet<Figurinomanequim> Figurinomanequims { get; set; } = null!;
        public virtual DbSet<Grupomusical> Grupomusicals { get; set; } = null!;
        public virtual DbSet<Instrumentomusical> Instrumentomusicals { get; set; } = null!;
        public virtual DbSet<Manequim> Manequims { get; set; } = null!;
        public virtual DbSet<Movimentacaoinstrumento> Movimentacaoinstrumentos { get; set; } = null!;
        public virtual DbSet<Papelgrupo> Papelgrupos { get; set; } = null!;
        public virtual DbSet<Pessoa> Pessoas { get; set; } = null!;
        public virtual DbSet<Tipoinstrumento> Tipoinstrumentos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=123456;database=GrupoMusical");
            }*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apresentacao>(entity =>
            {
                entity.ToTable("apresentacao");

                entity.HasIndex(e => e.IdGrupoMusical, "fk_Ensaio_GrupoMusical1_idx");

                entity.HasIndex(e => e.IdColaboradorResponsavel, "fk_Ensaio_Pessoa1_idx");

                entity.HasIndex(e => e.IdRegente, "fk_Ensaio_Pessoa2_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataHoraFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataHoraFim");

                entity.Property(e => e.DataHoraInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataHoraInicio");

                entity.Property(e => e.IdColaboradorResponsavel).HasColumnName("idColaboradorResponsavel");

                entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");

                entity.Property(e => e.IdRegente).HasColumnName("idRegente");

                entity.Property(e => e.Local)
                    .HasMaxLength(100)
                    .HasColumnName("local");

                entity.Property(e => e.Repertorio)
                    .HasMaxLength(1000)
                    .HasColumnName("repertorio");

                entity.HasOne(d => d.IdColaboradorResponsavelNavigation)
                    .WithMany(p => p.ApresentacaoIdColaboradorResponsavelNavigations)
                    .HasForeignKey(d => d.IdColaboradorResponsavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_Pessoa10");

                entity.HasOne(d => d.IdGrupoMusicalNavigation)
                    .WithMany(p => p.Apresentacaos)
                    .HasForeignKey(d => d.IdGrupoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_GrupoMusical10");

                entity.HasOne(d => d.IdRegenteNavigation)
                    .WithMany(p => p.ApresentacaoIdRegenteNavigations)
                    .HasForeignKey(d => d.IdRegente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_Pessoa20");
            });

            modelBuilder.Entity<Apresentacaopessoa>(entity =>
            {
                entity.HasKey(e => new { e.IdApresentacao, e.IdPessoa })
                    .HasName("PRIMARY");

                entity.ToTable("apresentacaopessoa");

                entity.HasIndex(e => e.IdApresentacao, "fk_ApresentacaoPessoa_Apresentacao1_idx");

                entity.HasIndex(e => e.IdPessoa, "fk_ApresentacaoPessoa_Pessoa1_idx");

                entity.HasIndex(e => e.IdTipoInstrumento, "fk_ApresentacaoPessoa_TipoInstrumento1_idx");

                entity.Property(e => e.IdApresentacao).HasColumnName("idApresentacao");

                entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");

                entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");

                entity.Property(e => e.JustificativaAceita).HasColumnName("justificativaAceita");

                entity.Property(e => e.JustificativaFalta)
                    .HasMaxLength(200)
                    .HasColumnName("justificativaFalta");

                entity.Property(e => e.Presente).HasColumnName("presente");

                entity.Property(e => e.Status)
                    .HasColumnType("enum('INSCRITO','DEFERIDO','INDEFERIDO')")
                    .HasColumnName("status")
                    .HasDefaultValueSql("'INSCRITO'");

                entity.HasOne(d => d.IdApresentacaoNavigation)
                    .WithMany(p => p.Apresentacaopessoas)
                    .HasForeignKey(d => d.IdApresentacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApresentacaoPessoa_Apresentacao1");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Apresentacaopessoas)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApresentacaoPessoa_Pessoa1");

                entity.HasOne(d => d.IdTipoInstrumentoNavigation)
                    .WithMany(p => p.Apresentacaopessoas)
                    .HasForeignKey(d => d.IdTipoInstrumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApresentacaoPessoa_TipoInstrumento1");
            });

            modelBuilder.Entity<Apresentacaotipoinstrumento>(entity =>
            {
                entity.HasKey(e => new { e.IdApresentacao, e.IdTipoInstrumento })
                    .HasName("PRIMARY");

                entity.ToTable("apresentacaotipoinstrumento");

                entity.HasIndex(e => e.IdApresentacao, "fk_ApresentacaoTipoInstrumento_Apresentacao1_idx");

                entity.HasIndex(e => e.IdTipoInstrumento, "fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx");

                entity.Property(e => e.IdApresentacao).HasColumnName("idApresentacao");

                entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");

                entity.Property(e => e.QuantidadeConfirmada).HasColumnName("quantidadeConfirmada");

                entity.Property(e => e.QuantidadePlanejada).HasColumnName("quantidadePlanejada");

                entity.HasOne(d => d.IdApresentacaoNavigation)
                    .WithMany(p => p.Apresentacaotipoinstrumentos)
                    .HasForeignKey(d => d.IdApresentacao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApresentacaoTipoInstrumento_Apresentacao1");

                entity.HasOne(d => d.IdTipoInstrumentoNavigation)
                    .WithMany(p => p.Apresentacaotipoinstrumentos)
                    .HasForeignKey(d => d.IdTipoInstrumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ApresentacaoTipoInstrumento_TipoInstrumento1");
            });

            modelBuilder.Entity<Ensaio>(entity =>
            {
                entity.ToTable("ensaio");

                entity.HasIndex(e => e.IdGrupoMusical, "fk_Ensaio_GrupoMusical1_idx");

                entity.HasIndex(e => e.IdColaboradorResponsavel, "fk_Ensaio_Pessoa1_idx");

                entity.HasIndex(e => e.IdRegente, "fk_Ensaio_Pessoa2_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataHoraFim)
                    .HasColumnType("datetime")
                    .HasColumnName("dataHoraFim");

                entity.Property(e => e.DataHoraInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("dataHoraInicio");

                entity.Property(e => e.IdColaboradorResponsavel).HasColumnName("idColaboradorResponsavel");

                entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");

                entity.Property(e => e.IdRegente).HasColumnName("idRegente");

                entity.Property(e => e.Local)
                    .HasMaxLength(100)
                    .HasColumnName("local");

                entity.Property(e => e.PresencaObrigatoria)
                    .HasColumnName("presencaObrigatoria")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Repertorio)
                    .HasMaxLength(1000)
                    .HasColumnName("repertorio");

                entity.Property(e => e.Tipo)
                    .HasColumnType("enum('FIXO','EXTRA')")
                    .HasColumnName("tipo")
                    .HasDefaultValueSql("'FIXO'");

                entity.HasOne(d => d.IdColaboradorResponsavelNavigation)
                    .WithMany(p => p.EnsaioIdColaboradorResponsavelNavigations)
                    .HasForeignKey(d => d.IdColaboradorResponsavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_Pessoa1");

                entity.HasOne(d => d.IdGrupoMusicalNavigation)
                    .WithMany(p => p.Ensaios)
                    .HasForeignKey(d => d.IdGrupoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_GrupoMusical1");

                entity.HasOne(d => d.IdRegenteNavigation)
                    .WithMany(p => p.EnsaioIdRegenteNavigations)
                    .HasForeignKey(d => d.IdRegente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Ensaio_Pessoa2");
            });

            modelBuilder.Entity<Ensaiopessoa>(entity =>
            {
                entity.HasKey(e => new { e.IdPessoa, e.IdEnsaio })
                    .HasName("PRIMARY");

                entity.ToTable("ensaiopessoa");

                entity.HasIndex(e => e.IdEnsaio, "fk_PessoaEnsaio_Ensaio1_idx");

                entity.HasIndex(e => e.IdPessoa, "fk_PessoaEnsaio_Pessoa1_idx");

                entity.Property(e => e.IdPessoa).HasColumnName("idPessoa");

                entity.Property(e => e.IdEnsaio).HasColumnName("idEnsaio");

                entity.Property(e => e.JustificativaAceita).HasColumnName("justificativaAceita");

                entity.Property(e => e.JustificativaFalta)
                    .HasMaxLength(200)
                    .HasColumnName("justificativaFalta");

                entity.Property(e => e.Presente).HasColumnName("presente");

                entity.HasOne(d => d.IdEnsaioNavigation)
                    .WithMany(p => p.Ensaiopessoas)
                    .HasForeignKey(d => d.IdEnsaio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PessoaEnsaio_Ensaio1");

                entity.HasOne(d => d.IdPessoaNavigation)
                    .WithMany(p => p.Ensaiopessoas)
                    .HasForeignKey(d => d.IdPessoa)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PessoaEnsaio_Pessoa1");
            });

            modelBuilder.Entity<Entregarfigurino>(entity =>
            {
                entity.ToTable("entregarfigurino");

                entity.HasIndex(e => e.IdFigurino, "fk_EntregarFigurino_Figurino1_idx");

                entity.HasIndex(e => e.IdManequim, "fk_EntregarFigurino_Manequim1_idx");

                entity.HasIndex(e => e.IdAssociado, "fk_EntregarFigurino_Pessoa1_idx");

                entity.HasIndex(e => e.IdColaborador, "fk_EntregarFigurino_Pessoa2_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConfirmacaoAssociado).HasColumnName("confirmacaoAssociado");

                entity.Property(e => e.Data)
                    .HasColumnType("datetime")
                    .HasColumnName("data");

                entity.Property(e => e.IdAssociado).HasColumnName("idAssociado");

                entity.Property(e => e.IdColaborador).HasColumnName("idColaborador");

                entity.Property(e => e.IdFigurino).HasColumnName("idFigurino");

                entity.Property(e => e.IdManequim).HasColumnName("idManequim");

                entity.Property(e => e.Status)
                    .HasColumnType("enum('DISPONIVEL','ENTREGUE','RECEBIDO','DANIFICADO','DEVOLVIDO')")
                    .HasColumnName("status")
                    .HasDefaultValueSql("'DISPONIVEL'");

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.EntregarfigurinoIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_EntregarFigurino_Pessoa1");

                entity.HasOne(d => d.IdColaboradorNavigation)
                    .WithMany(p => p.EntregarfigurinoIdColaboradorNavigations)
                    .HasForeignKey(d => d.IdColaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_EntregarFigurino_Pessoa2");

                entity.HasOne(d => d.IdFigurinoNavigation)
                    .WithMany(p => p.Entregarfigurinos)
                    .HasForeignKey(d => d.IdFigurino)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_EntregarFigurino_Figurino1");

                entity.HasOne(d => d.IdManequimNavigation)
                    .WithMany(p => p.Entregarfigurinos)
                    .HasForeignKey(d => d.IdManequim)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_EntregarFigurino_Manequim1");
            });

            modelBuilder.Entity<Figurino>(entity =>
            {
                entity.ToTable("figurino");

                entity.HasIndex(e => e.IdGrupoMusical, "fk_Figurino_GrupoMusical1_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Data)
                    .HasColumnType("date")
                    .HasColumnName("data");

                entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .HasColumnName("nome");

                entity.HasOne(d => d.IdGrupoMusicalNavigation)
                    .WithMany(p => p.Figurinos)
                    .HasForeignKey(d => d.IdGrupoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Figurino_GrupoMusical1");

                entity.HasMany(d => d.IdApresentacaos)
                    .WithMany(p => p.IdFigurinos)
                    .UsingEntity<Dictionary<string, object>>(
                        "Figurinoapresentacao",
                        l => l.HasOne<Apresentacao>().WithMany().HasForeignKey("IdApresentacao").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_FigurinoApresentacao_Apresentacao1"),
                        r => r.HasOne<Figurino>().WithMany().HasForeignKey("IdFigurino").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_FigurinoApresentacao_Figurino1"),
                        j =>
                        {
                            j.HasKey("IdFigurino", "IdApresentacao").HasName("PRIMARY");

                            j.ToTable("figurinoapresentacao");

                            j.HasIndex(new[] { "IdApresentacao" }, "fk_FigurinoApresentacao_Apresentacao1_idx");

                            j.HasIndex(new[] { "IdFigurino" }, "fk_FigurinoApresentacao_Figurino1_idx");

                            j.IndexerProperty<int>("IdFigurino").HasColumnName("idFigurino");

                            j.IndexerProperty<int>("IdApresentacao").HasColumnName("idApresentacao");
                        });

                entity.HasMany(d => d.IdEnsaios)
                    .WithMany(p => p.IdFigurinos)
                    .UsingEntity<Dictionary<string, object>>(
                        "Figurinoensaio",
                        l => l.HasOne<Ensaio>().WithMany().HasForeignKey("IdEnsaio").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_FigurinoEnsaio_Ensaio1"),
                        r => r.HasOne<Figurino>().WithMany().HasForeignKey("IdFigurino").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_FigurinoEnsaio_Figurino1"),
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
                entity.HasKey(e => new { e.IdFigurino, e.IdManequim })
                    .HasName("PRIMARY");

                entity.ToTable("figurinomanequim");

                entity.HasIndex(e => e.IdFigurino, "fk_FigurinoManequim_Figurino1_idx");

                entity.HasIndex(e => e.IdManequim, "fk_FigurinoManequim_Manequim1_idx");

                entity.Property(e => e.IdFigurino).HasColumnName("idFigurino");

                entity.Property(e => e.IdManequim).HasColumnName("idManequim");

                entity.Property(e => e.QuantidadeDisponivel).HasColumnName("quantidadeDisponivel");

                entity.Property(e => e.QuantidadeEntregue).HasColumnName("quantidadeEntregue");

                entity.HasOne(d => d.IdFigurinoNavigation)
                    .WithMany(p => p.Figurinomanequims)
                    .HasForeignKey(d => d.IdFigurino)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_FigurinoManequim_Figurino1");

                entity.HasOne(d => d.IdManequimNavigation)
                    .WithMany(p => p.Figurinomanequims)
                    .HasForeignKey(d => d.IdManequim)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_FigurinoManequim_Manequim1");
            });

            modelBuilder.Entity<Grupomusical>(entity =>
            {
                entity.ToTable("grupomusical");

                entity.HasIndex(e => e.Cnpj, "cnpj_UNIQUE")
                    .IsUnique();

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
                    .HasColumnName("estado")
                    .HasDefaultValueSql("'SE'")
                    .IsFixedLength();

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

            modelBuilder.Entity<Instrumentomusical>(entity =>
            {
                entity.ToTable("instrumentomusical");

                entity.HasIndex(e => e.IdGrupoMusical, "fk_InstrumentoMusical_GrupoMusical1_idx");

                entity.HasIndex(e => e.IdTipoInstrumento, "fk_InstrumentoMusical_TipoInstrumento1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DataAquisicao)
                    .HasColumnType("date")
                    .HasColumnName("dataAquisicao");

                entity.Property(e => e.IdGrupoMusical).HasColumnName("idGrupoMusical");

                entity.Property(e => e.IdTipoInstrumento).HasColumnName("idTipoInstrumento");

                entity.Property(e => e.Status)
                    .HasColumnType("enum('DISPONIVEL','EMPRESTADO','DANIFICADO')")
                    .HasColumnName("status")
                    .HasDefaultValueSql("'DISPONIVEL'");

                entity.HasOne(d => d.IdGrupoMusicalNavigation)
                    .WithMany(p => p.Instrumentomusicals)
                    .HasForeignKey(d => d.IdGrupoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_InstrumentoMusical_GrupoMusical1");

                entity.HasOne(d => d.IdTipoInstrumentoNavigation)
                    .WithMany(p => p.Instrumentomusicals)
                    .HasForeignKey(d => d.IdTipoInstrumento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_InstrumentoMusical_TipoInstrumento1");
            });

            modelBuilder.Entity<Manequim>(entity =>
            {
                entity.ToTable("manequim");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descricao)
                    .HasMaxLength(100)
                    .HasColumnName("descricao");

                entity.Property(e => e.Tamanho)
                    .HasMaxLength(2)
                    .HasColumnName("tamanho");
            });

            modelBuilder.Entity<Movimentacaoinstrumento>(entity =>
            {
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
                    .HasColumnType("enum('EMPRESTIMO','DEVOLUCAO')")
                    .HasColumnName("tipoMovimento")
                    .HasDefaultValueSql("'EMPRESTIMO'");

                entity.HasOne(d => d.IdAssociadoNavigation)
                    .WithMany(p => p.MovimentacaoinstrumentoIdAssociadoNavigations)
                    .HasForeignKey(d => d.IdAssociado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_DevolverInstrumento_Pessoa1");

                entity.HasOne(d => d.IdColaboradorNavigation)
                    .WithMany(p => p.MovimentacaoinstrumentoIdColaboradorNavigations)
                    .HasForeignKey(d => d.IdColaborador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_DevolverInstrumento_Pessoa2");

                entity.HasOne(d => d.IdInstrumentoMusicalNavigation)
                    .WithMany(p => p.Movimentacaoinstrumentos)
                    .HasForeignKey(d => d.IdInstrumentoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_DevolverInstrumento_InstrumentoMusical1");
            });

            modelBuilder.Entity<Papelgrupo>(entity =>
            {
                entity.HasKey(e => e.IdPapelGrupo)
                    .HasName("PRIMARY");

                entity.ToTable("papelgrupo");

                entity.Property(e => e.IdPapelGrupo).HasColumnName("idPapelGrupo");

                entity.Property(e => e.Nome)
                    .HasMaxLength(100)
                    .HasColumnName("nome");
            });

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.ToTable("pessoa");

                entity.HasIndex(e => e.Cpf, "cpf_UNIQUE")
                    .IsUnique();

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
                    .HasColumnName("estado")
                    .HasDefaultValueSql("'SE'")
                    .IsFixedLength();

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
                    .HasColumnType("enum('M','F')")
                    .HasColumnName("sexo")
                    .HasDefaultValueSql("'F'");

                entity.Property(e => e.Telefone1)
                    .HasMaxLength(20)
                    .HasColumnName("telefone1");

                entity.Property(e => e.Telefone2)
                    .HasMaxLength(20)
                    .HasColumnName("telefone2");

                entity.HasOne(d => d.IdGrupoMusicalNavigation)
                    .WithMany(p => p.Pessoas)
                    .HasForeignKey(d => d.IdGrupoMusical)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Pessoa_GrupoMusical1");

                entity.HasOne(d => d.IdManequimNavigation)
                    .WithMany(p => p.Pessoas)
                    .HasForeignKey(d => d.IdManequim)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Pessoa_Manequim1");

                entity.HasOne(d => d.IdPapelGrupoNavigation)
                    .WithMany(p => p.Pessoas)
                    .HasForeignKey(d => d.IdPapelGrupo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Pessoa_PapelGrupoMusical1");

                entity.HasMany(d => d.IdTipoInstrumentos)
                    .WithMany(p => p.IdPessoas)
                    .UsingEntity<Dictionary<string, object>>(
                        "Pessoatipoinstrumento",
                        l => l.HasOne<Tipoinstrumento>().WithMany().HasForeignKey("IdTipoInstrumento").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_Pessoa_has_TipoInstrumento_TipoInstrumento1"),
                        r => r.HasOne<Pessoa>().WithMany().HasForeignKey("IdPessoa").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_Pessoa_has_TipoInstrumento_Pessoa"),
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

            modelBuilder.Entity<Tipoinstrumento>(entity =>
            {
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
}
