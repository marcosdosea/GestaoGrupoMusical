using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaNumeroComplementoEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "complemento",
                table: "pessoa",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero",
                table: "pessoa",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "complemento",
                table: "grupomusical",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero",
                table: "grupomusical",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "complemento",
                table: "pessoa");

            migrationBuilder.DropColumn(
                name: "numero",
                table: "pessoa");

            migrationBuilder.DropColumn(
                name: "complemento",
                table: "grupomusical");

            migrationBuilder.DropColumn(
                name: "numero",
                table: "grupomusical");
        }
    }
}
