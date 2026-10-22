using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaEventoEPessoaTipoInstrumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "prioridadeEscolha",
                table: "pessoatipoinstrumento",
                type: "int",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AlterColumn<string>(
                name: "repertorio",
                table: "evento",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dataHoraChegadaEvento",
                table: "evento",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observacaoHoraChegada",
                table: "evento",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "precisaConfirmarParticipacao",
                table: "evento",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prioridadeEscolha",
                table: "pessoatipoinstrumento");

            migrationBuilder.DropColumn(
                name: "dataHoraChegadaEvento",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "observacaoHoraChegada",
                table: "evento");

            migrationBuilder.DropColumn(
                name: "precisaConfirmarParticipacao",
                table: "evento");

            migrationBuilder.AlterColumn<string>(
                name: "repertorio",
                table: "evento",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(5000)",
                oldMaxLength: 5000,
                oldNullable: true);
        }
    }
}
