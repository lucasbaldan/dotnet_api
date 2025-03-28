using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_api.Database.Migrations
{
    /// <inheritdoc />
    public partial class adicionandoPermissoesGrupoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PermissoesJson",
                table: "grupos_usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissoesJson",
                table: "grupos_usuarios");
        }
    }
}
