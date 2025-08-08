using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarPapelGrupoNaPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE pessoa SET idPapelGrupo = (SELECT idPapelGrupo FROM papelgrupo WHERE nome = 'ADMINISTRADOR SISTEMA') WHERE email = 'caioteste949@gmail.com';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE pessoa SET idPapelGrupo = (SELECT idPapelGrupo FROM papelgrupo WHERE nome = 'REGENTE') WHERE email = 'caioteste949@gmail.com';");
        }
    }
}
