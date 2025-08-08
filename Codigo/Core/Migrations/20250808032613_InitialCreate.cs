using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetroles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetusers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<ulong>(type: "bit(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<ulong>(type: "bit(1)", nullable: false),
                    TwoFactorEnabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "timestamp", nullable: true),
                    LockoutEnabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "grupomusical",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    razaoSocial = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    cnpj = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    cep = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true),
                    rua = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: false, defaultValueSql: "'SE'"),
                    pais = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    youtube = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    instagram = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    facebook = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    telefone1 = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    telefone2 = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    banco = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    agencia = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    numeroContaBanco = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    chavePIX = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    chavePIXTipo = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "manequim",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tamanho = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "papelgrupo",
                columns: table => new
                {
                    idPapelGrupo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.idPapelGrupo);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipoinstrumento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetroleclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserlogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserroles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    RoleId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetusertokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(767)", maxLength: 767, nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "figurino",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    data = table.Column<DateTime>(type: "date", nullable: true),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Figurino_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receitafinanceira",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    descricao = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    dataInicio = table.Column<DateTime>(type: "date", nullable: false),
                    dataFim = table.Column<DateTime>(type: "date", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_ReceitaFinanceira_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pessoa",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    cpf = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    sexo = table.Column<string>(type: "enum('M','F')", nullable: false, defaultValueSql: "'F'"),
                    cep = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    rua = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    cidade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "char(2)", fixedLength: true, maxLength: 2, nullable: false, defaultValueSql: "'SE'"),
                    dataNascimento = table.Column<DateTime>(type: "date", nullable: true),
                    telefone1 = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    telefone2 = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    dataEntrada = table.Column<DateTime>(type: "date", nullable: true),
                    dataSaida = table.Column<DateTime>(type: "date", nullable: true),
                    motivoSaida = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ativo = table.Column<sbyte>(type: "tinyint", nullable: false),
                    isentoPagamento = table.Column<sbyte>(type: "tinyint", nullable: false),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false),
                    idPapelGrupo = table.Column<int>(type: "int", nullable: false),
                    idManequim = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Pessoa_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Pessoa_Manequim1",
                        column: x => x.idManequim,
                        principalTable: "manequim",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Pessoa_PapelGrupoMusical1",
                        column: x => x.idPapelGrupo,
                        principalTable: "papelgrupo",
                        principalColumn: "idPapelGrupo");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "instrumentomusical",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    patrimonio = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    dataAquisicao = table.Column<DateTime>(type: "date", nullable: false),
                    status = table.Column<string>(type: "enum('DISPONIVEL','EMPRESTADO','DANIFICADO')", nullable: false, defaultValueSql: "'DISPONIVEL'"),
                    idTipoInstrumento = table.Column<int>(type: "int", nullable: false),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_InstrumentoMusical_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_InstrumentoMusical_TipoInstrumento1",
                        column: x => x.idTipoInstrumento,
                        principalTable: "tipoinstrumento",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "figurinomanequim",
                columns: table => new
                {
                    idFigurino = table.Column<int>(type: "int", nullable: false),
                    idManequim = table.Column<int>(type: "int", nullable: false),
                    quantidadeDisponivel = table.Column<int>(type: "int", nullable: false),
                    quantidadeEntregue = table.Column<int>(type: "int", nullable: false),
                    quantidadeDescartada = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idFigurino, x.idManequim });
                    table.ForeignKey(
                        name: "fk_FigurinoManequim_Figurino1",
                        column: x => x.idFigurino,
                        principalTable: "figurino",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_FigurinoManequim_Manequim1",
                        column: x => x.idManequim,
                        principalTable: "manequim",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ensaio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "enum('FIXO','EXTRA')", nullable: false, defaultValueSql: "'FIXO'"),
                    dataHoraInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataHoraFim = table.Column<DateTime>(type: "datetime", nullable: false),
                    presencaObrigatoria = table.Column<sbyte>(type: "tinyint", nullable: false),
                    local = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    repertorio = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    idColaboradorResponsavel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Ensaio_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Ensaio_Pessoa1",
                        column: x => x.idColaboradorResponsavel,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "evento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false),
                    dataHoraInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    dataHoraFim = table.Column<DateTime>(type: "datetime", nullable: false),
                    local = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    repertorio = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    idColaboradorResponsavel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_Ensaio_GrupoMusical10",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Ensaio_Pessoa10",
                        column: x => x.idColaboradorResponsavel,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "informativo",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false),
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    mensagem = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    data = table.Column<DateTime>(type: "datetime", nullable: false),
                    entregarAssociadosAtivos = table.Column<sbyte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_GrupoMusicalPessoa_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_GrupoMusicalPessoa_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "materialestudo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    link = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    data = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    idGrupoMusical = table.Column<int>(type: "int", nullable: false),
                    idColaborador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_MaterialEstudo_GrupoMusical1",
                        column: x => x.idGrupoMusical,
                        principalTable: "grupomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_MaterialEstudo_Pessoa1",
                        column: x => x.idColaborador,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "movimentacaofigurino",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateTime>(type: "datetime", nullable: false),
                    idFigurino = table.Column<int>(type: "int", nullable: false),
                    idManequim = table.Column<int>(type: "int", nullable: false),
                    idAssociado = table.Column<int>(type: "int", nullable: false),
                    idColaborador = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "enum('DISPONIVEL','ENTREGUE','RECEBIDO','DANIFICADO','DEVOLVIDO')", nullable: false, defaultValueSql: "'DISPONIVEL'"),
                    confirmacaoRecebimento = table.Column<sbyte>(type: "tinyint", nullable: false),
                    quantidade = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_EntregarFigurino_Figurino1",
                        column: x => x.idFigurino,
                        principalTable: "figurino",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_EntregarFigurino_Manequim1",
                        column: x => x.idManequim,
                        principalTable: "manequim",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_EntregarFigurino_Pessoa1",
                        column: x => x.idAssociado,
                        principalTable: "pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_EntregarFigurino_Pessoa2",
                        column: x => x.idColaborador,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pessoatipoinstrumento",
                columns: table => new
                {
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    idTipoInstrumento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idPessoa, x.idTipoInstrumento });
                    table.ForeignKey(
                        name: "fk_Pessoa_has_TipoInstrumento_Pessoa",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_Pessoa_has_TipoInstrumento_TipoInstrumento1",
                        column: x => x.idTipoInstrumento,
                        principalTable: "tipoinstrumento",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "receitafinanceirapessoa",
                columns: table => new
                {
                    idReceitaFinanceira = table.Column<int>(type: "int", nullable: false),
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    valorPago = table.Column<decimal>(type: "decimal(10,2)", precision: 10, nullable: false),
                    dataPagamento = table.Column<DateTime>(type: "datetime", nullable: false),
                    observacoes = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "enum('ABERTO','ENVIADO','PAGO','ISENTO')", nullable: false, defaultValueSql: "'ABERTO'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idReceitaFinanceira, x.idPessoa });
                    table.ForeignKey(
                        name: "fk_ReceitaFinanceiraPessoa_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1",
                        column: x => x.idReceitaFinanceira,
                        principalTable: "receitafinanceira",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "movimentacaoinstrumento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    data = table.Column<DateTime>(type: "datetime", nullable: false),
                    idInstrumentoMusical = table.Column<int>(type: "int", nullable: false),
                    idAssociado = table.Column<int>(type: "int", nullable: false),
                    idColaborador = table.Column<int>(type: "int", nullable: false),
                    confirmacaoAssociado = table.Column<sbyte>(type: "tinyint", nullable: false),
                    tipoMovimento = table.Column<string>(type: "enum('EMPRESTIMO','DEVOLUCAO')", nullable: false, defaultValueSql: "'EMPRESTIMO'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_DevolverInstrumento_InstrumentoMusical1",
                        column: x => x.idInstrumentoMusical,
                        principalTable: "instrumentomusical",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_DevolverInstrumento_Pessoa1",
                        column: x => x.idAssociado,
                        principalTable: "pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_DevolverInstrumento_Pessoa2",
                        column: x => x.idColaborador,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ensaiopessoa",
                columns: table => new
                {
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    idEnsaio = table.Column<int>(type: "int", nullable: false),
                    presente = table.Column<sbyte>(type: "tinyint", nullable: false),
                    justificativaFalta = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    justificativaAceita = table.Column<sbyte>(type: "tinyint", nullable: false),
                    idPapelGrupo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idPessoa, x.idEnsaio });
                    table.ForeignKey(
                        name: "fk_EnsaioPessoa_PapelGrupo1",
                        column: x => x.idPapelGrupo,
                        principalTable: "papelgrupo",
                        principalColumn: "idPapelGrupo");
                    table.ForeignKey(
                        name: "fk_PessoaEnsaio_Ensaio1",
                        column: x => x.idEnsaio,
                        principalTable: "ensaio",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_PessoaEnsaio_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "figurinoensaio",
                columns: table => new
                {
                    idFigurino = table.Column<int>(type: "int", nullable: false),
                    idEnsaio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idFigurino, x.idEnsaio });
                    table.ForeignKey(
                        name: "fk_FigurinoEnsaio_Ensaio1",
                        column: x => x.idEnsaio,
                        principalTable: "ensaio",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_FigurinoEnsaio_Figurino1",
                        column: x => x.idFigurino,
                        principalTable: "figurino",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "apresentacaotipoinstrumento",
                columns: table => new
                {
                    idApresentacao = table.Column<int>(type: "int", nullable: false),
                    idTipoInstrumento = table.Column<int>(type: "int", nullable: false),
                    quantidadePlanejada = table.Column<int>(type: "int", nullable: false),
                    quantidadeConfirmada = table.Column<int>(type: "int", nullable: false),
                    quantidadeSolicitada = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idApresentacao, x.idTipoInstrumento });
                    table.ForeignKey(
                        name: "fk_ApresentacaoTipoInstrumento_Apresentacao1",
                        column: x => x.idApresentacao,
                        principalTable: "evento",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ApresentacaoTipoInstrumento_TipoInstrumento1",
                        column: x => x.idTipoInstrumento,
                        principalTable: "tipoinstrumento",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "eventopessoa",
                columns: table => new
                {
                    idEvento = table.Column<int>(type: "int", nullable: false),
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    idTipoInstrumento = table.Column<int>(type: "int", nullable: false),
                    presente = table.Column<sbyte>(type: "tinyint", nullable: false),
                    justificativaFalta = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    justificativaAceita = table.Column<sbyte>(type: "tinyint", nullable: false),
                    status = table.Column<string>(type: "enum('INSCRITO','DEFERIDO','INDEFERIDO')", nullable: false, defaultValueSql: "'INSCRITO'"),
                    idPapelGrupoPapelGrupo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idEvento, x.idPessoa });
                    table.ForeignKey(
                        name: "fk_ApresentacaoPessoa_Apresentacao1",
                        column: x => x.idEvento,
                        principalTable: "evento",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ApresentacaoPessoa_Pessoa1",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ApresentacaoPessoa_TipoInstrumento1",
                        column: x => x.idTipoInstrumento,
                        principalTable: "tipoinstrumento",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_EventoPessoa_PapelGrupo1",
                        column: x => x.idPapelGrupoPapelGrupo,
                        principalTable: "papelgrupo",
                        principalColumn: "idPapelGrupo");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "figurinoapresentacao",
                columns: table => new
                {
                    idFigurino = table.Column<int>(type: "int", nullable: false),
                    idApresentacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.idFigurino, x.idApresentacao });
                    table.ForeignKey(
                        name: "fk_FigurinoApresentacao_Apresentacao1",
                        column: x => x.idApresentacao,
                        principalTable: "evento",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_FigurinoApresentacao_Figurino1",
                        column: x => x.idFigurino,
                        principalTable: "figurino",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_ApresentacaoTipoInstrumento_Apresentacao1_idx",
                table: "apresentacaotipoinstrumento",
                column: "idApresentacao");

            migrationBuilder.CreateIndex(
                name: "fk_ApresentacaoTipoInstrumento_TipoInstrumento1_idx",
                table: "apresentacaotipoinstrumento",
                column: "idTipoInstrumento");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "aspnetroleclaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "aspnetroles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "aspnetuserclaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "aspnetuserlogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "aspnetuserroles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "aspnetusers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "aspnetusers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Ensaio_GrupoMusical1_idx",
                table: "ensaio",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_Ensaio_Pessoa1_idx",
                table: "ensaio",
                column: "idColaboradorResponsavel");

            migrationBuilder.CreateIndex(
                name: "fk_EnsaioPessoa_PapelGrupo1_idx",
                table: "ensaiopessoa",
                column: "idPapelGrupo");

            migrationBuilder.CreateIndex(
                name: "fk_PessoaEnsaio_Ensaio1_idx",
                table: "ensaiopessoa",
                column: "idEnsaio");

            migrationBuilder.CreateIndex(
                name: "fk_PessoaEnsaio_Pessoa1_idx",
                table: "ensaiopessoa",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_Ensaio_GrupoMusical1_idx1",
                table: "evento",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_Ensaio_Pessoa1_idx1",
                table: "evento",
                column: "idColaboradorResponsavel");

            migrationBuilder.CreateIndex(
                name: "fk_ApresentacaoPessoa_Apresentacao1_idx",
                table: "eventopessoa",
                column: "idEvento");

            migrationBuilder.CreateIndex(
                name: "fk_ApresentacaoPessoa_Pessoa1_idx",
                table: "eventopessoa",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_ApresentacaoPessoa_TipoInstrumento1_idx",
                table: "eventopessoa",
                column: "idTipoInstrumento");

            migrationBuilder.CreateIndex(
                name: "fk_EventoPessoa_PapelGrupo1_idx",
                table: "eventopessoa",
                column: "idPapelGrupoPapelGrupo");

            migrationBuilder.CreateIndex(
                name: "fk_Figurino_GrupoMusical1_idx",
                table: "figurino",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "figurino",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoApresentacao_Apresentacao1_idx",
                table: "figurinoapresentacao",
                column: "idApresentacao");

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoApresentacao_Figurino1_idx",
                table: "figurinoapresentacao",
                column: "idFigurino");

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoEnsaio_Ensaio1_idx",
                table: "figurinoensaio",
                column: "idEnsaio");

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoEnsaio_Figurino1_idx",
                table: "figurinoensaio",
                column: "idFigurino");

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoManequim_Figurino1_idx",
                table: "figurinomanequim",
                column: "idFigurino");

            migrationBuilder.CreateIndex(
                name: "fk_FigurinoManequim_Manequim1_idx",
                table: "figurinomanequim",
                column: "idManequim");

            migrationBuilder.CreateIndex(
                name: "cnpj_UNIQUE",
                table: "grupomusical",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_GrupoMusicalPessoa_GrupoMusical1_idx",
                table: "informativo",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_GrupoMusicalPessoa_Pessoa1_idx",
                table: "informativo",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_InstrumentoMusical_GrupoMusical1_idx",
                table: "instrumentomusical",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_InstrumentoMusical_TipoInstrumento1_idx",
                table: "instrumentomusical",
                column: "idTipoInstrumento");

            migrationBuilder.CreateIndex(
                name: "fk_MaterialEstudo_GrupoMusical1_idx",
                table: "materialestudo",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_MaterialEstudo_Pessoa1_idx",
                table: "materialestudo",
                column: "idColaborador");

            migrationBuilder.CreateIndex(
                name: "fk_EntregarFigurino_Figurino1_idx",
                table: "movimentacaofigurino",
                column: "idFigurino");

            migrationBuilder.CreateIndex(
                name: "fk_EntregarFigurino_Manequim1_idx",
                table: "movimentacaofigurino",
                column: "idManequim");

            migrationBuilder.CreateIndex(
                name: "fk_EntregarFigurino_Pessoa1_idx",
                table: "movimentacaofigurino",
                column: "idAssociado");

            migrationBuilder.CreateIndex(
                name: "fk_EntregarFigurino_Pessoa2_idx",
                table: "movimentacaofigurino",
                column: "idColaborador");

            migrationBuilder.CreateIndex(
                name: "fk_DevolverInstrumento_InstrumentoMusical1_idx",
                table: "movimentacaoinstrumento",
                column: "idInstrumentoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_DevolverInstrumento_Pessoa1_idx",
                table: "movimentacaoinstrumento",
                column: "idAssociado");

            migrationBuilder.CreateIndex(
                name: "fk_DevolverInstrumento_Pessoa2_idx",
                table: "movimentacaoinstrumento",
                column: "idColaborador");

            migrationBuilder.CreateIndex(
                name: "cpf_UNIQUE",
                table: "pessoa",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_GrupoMusical1_idx",
                table: "pessoa",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_Manequim1_idx",
                table: "pessoa",
                column: "idManequim");

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_PapelGrupoMusical1_idx",
                table: "pessoa",
                column: "idPapelGrupo");

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_has_TipoInstrumento_Pessoa_idx",
                table: "pessoatipoinstrumento",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_Pessoa_has_TipoInstrumento_TipoInstrumento1_idx",
                table: "pessoatipoinstrumento",
                column: "idTipoInstrumento");

            migrationBuilder.CreateIndex(
                name: "fk_ReceitaFinanceira_GrupoMusical1_idx",
                table: "receitafinanceira",
                column: "idGrupoMusical");

            migrationBuilder.CreateIndex(
                name: "fk_ReceitaFinanceiraPessoa_Pessoa1_idx",
                table: "receitafinanceirapessoa",
                column: "idPessoa");

            migrationBuilder.CreateIndex(
                name: "fk_ReceitaFinanceiraPessoa_ReceitaFinanceira1_idx",
                table: "receitafinanceirapessoa",
                column: "idReceitaFinanceira");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apresentacaotipoinstrumento");

            migrationBuilder.DropTable(
                name: "aspnetroleclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserlogins");

            migrationBuilder.DropTable(
                name: "aspnetuserroles");

            migrationBuilder.DropTable(
                name: "aspnetusertokens");

            migrationBuilder.DropTable(
                name: "ensaiopessoa");

            migrationBuilder.DropTable(
                name: "eventopessoa");

            migrationBuilder.DropTable(
                name: "figurinoapresentacao");

            migrationBuilder.DropTable(
                name: "figurinoensaio");

            migrationBuilder.DropTable(
                name: "figurinomanequim");

            migrationBuilder.DropTable(
                name: "informativo");

            migrationBuilder.DropTable(
                name: "materialestudo");

            migrationBuilder.DropTable(
                name: "movimentacaofigurino");

            migrationBuilder.DropTable(
                name: "movimentacaoinstrumento");

            migrationBuilder.DropTable(
                name: "pessoatipoinstrumento");

            migrationBuilder.DropTable(
                name: "receitafinanceirapessoa");

            migrationBuilder.DropTable(
                name: "aspnetroles");

            migrationBuilder.DropTable(
                name: "aspnetusers");

            migrationBuilder.DropTable(
                name: "evento");

            migrationBuilder.DropTable(
                name: "ensaio");

            migrationBuilder.DropTable(
                name: "figurino");

            migrationBuilder.DropTable(
                name: "instrumentomusical");

            migrationBuilder.DropTable(
                name: "receitafinanceira");

            migrationBuilder.DropTable(
                name: "pessoa");

            migrationBuilder.DropTable(
                name: "tipoinstrumento");

            migrationBuilder.DropTable(
                name: "grupomusical");

            migrationBuilder.DropTable(
                name: "manequim");

            migrationBuilder.DropTable(
                name: "papelgrupo");
        }
    }
}
