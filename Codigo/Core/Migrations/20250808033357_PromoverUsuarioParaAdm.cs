using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class PromoverUsuarioParaAdm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE aspnetuserroles SET RoleId = (SELECT Id FROM aspnetroles WHERE Name = 'ADMINISTRADOR SISTEMA') WHERE UserId = (SELECT Id FROM aspnetusers WHERE Email = 'caioteste949@gmail.com');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE aspnetuserroles SET RoleId = (SELECT Id FROM aspnetroles WHERE Name = 'REGENTE') WHERE UserId = (SELECT Id FROM aspnetusers WHERE Email = 'caioteste949@gmail.com');");
        }
    }
}
