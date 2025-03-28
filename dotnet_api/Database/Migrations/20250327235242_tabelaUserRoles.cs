using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_api.Database.Migrations
{
    /// <inheritdoc />
    public partial class tabelaUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_grupos_usuarios_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_usuarios_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "usuarios_grupo_usuarios");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "usuarios_grupo_usuarios",
                newName: "IX_usuarios_grupo_usuarios_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios_grupo_usuarios",
                table: "usuarios_grupo_usuarios",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_grupo_usuarios_grupos_usuarios_RoleId",
                table: "usuarios_grupo_usuarios",
                column: "RoleId",
                principalTable: "grupos_usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_grupo_usuarios_usuarios_UserId",
                table: "usuarios_grupo_usuarios",
                column: "UserId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_grupo_usuarios_grupos_usuarios_RoleId",
                table: "usuarios_grupo_usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_grupo_usuarios_usuarios_UserId",
                table: "usuarios_grupo_usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios_grupo_usuarios",
                table: "usuarios_grupo_usuarios");

            migrationBuilder.RenameTable(
                name: "usuarios_grupo_usuarios",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_usuarios_grupo_usuarios_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_grupos_usuarios_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "grupos_usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_usuarios_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
