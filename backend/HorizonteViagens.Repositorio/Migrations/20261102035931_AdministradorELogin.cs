// ALTERAÇÃO PIM: Evolução do esquema, metadados EF e execução idempotente de SQL; arquivos gerados identificados.
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonteViagens.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class AdministradorELogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Clientes",
                type: "nchar(11)",
                fixedLength: true,
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Clientes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UltimoAcesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Ativo", "Email", "Nome", "SenhaHash", "UltimoAcesso" },
                values: new object[] { 1, true, "admin@horizonteviagens.com", "Administrador", "100000.R03eHwaH3+lUVmH+AEHXwg==.sPZ3h/VnYltKhLwXGC43dpu9Ck/HZ65KF4i87G7148g=", null });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_Email",
                table: "Administradores",
                column: "Email",
                unique: true);
            // Escrito à mão: a procedure passa a devolver o Celular.
            migrationBuilder.Sql(@"
ALTER PROCEDURE sp_Clientes_BuscarPorNome
    @Trecho NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, Cpf, Celular, DataCadastro, DataAtualizacao, Ativo
      FROM Clientes
     WHERE Nome LIKE '%' + @Trecho + '%'
     ORDER BY Nome;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER PROCEDURE sp_Clientes_BuscarPorNome
    @Trecho NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Email, Cpf, DataCadastro, DataAtualizacao, Ativo
      FROM Clientes
     WHERE Nome LIKE '%' + @Trecho + '%'
     ORDER BY Nome;
END");
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Clientes");
        }
    }
}
