// ALTERAÇÃO PIM: Evolução do esquema, metadados EF e execução idempotente de SQL; arquivos gerados identificados.
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonteViagens.Repositorio.Migrations
{
    /// <summary>
    /// Acrescenta a coluna RowVersion em Combos, para a concorrência otimista da compra
    /// (dois clientes tentando a última vaga ao mesmo tempo).
    /// </summary>
    public partial class ConcorrenciaCombos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // rowversion: o próprio SQL Server preenche e muda a cada alteração da linha.
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Combos",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Combos");
        }
    }
}
