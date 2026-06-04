using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTabelaDispositivoPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "dispositivopessoa",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    idPessoa = table.Column<int>(type: "int", nullable: false),
                    fcmToken = table.Column<string>(type: "text", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "fk_DispositivoPessoa_Pessoa",
                        column: x => x.idPessoa,
                        principalTable: "pessoa",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "fk_DispositivoPessoa_Pessoa_idx",
                table: "dispositivopessoa",
                column: "idPessoa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dispositivopessoa");

        }
    }
}
