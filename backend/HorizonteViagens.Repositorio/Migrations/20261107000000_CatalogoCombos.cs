using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonteViagens.Repositorio.Migrations
{
    /// <summary>
    /// Amplia Combos para a vitrine (nome, destino, vagas, categoria), cria a procedure
    /// de busca por destino e torna Compras.ComboId obrigatório.
    /// </summary>
    public partial class CatalogoCombos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------- Colunas da vitrine ----------
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Combos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Combo");

            migrationBuilder.AddColumn<string>(
                name: "Destino",
                table: "Combos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "A definir");

            migrationBuilder.AddColumn<int>(
                name: "Vagas",
                table: "Combos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Combos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Geral");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Combos_Vagas",
                table: "Combos",
                sql: "[Vagas] >= 0");

            // ---------- Procedure própria (escrita à mão: o EF não gera procedures) ----------
            // ALTERAÇÃO PIM: EXEC isola o CREATE no primeiro comando do lote, inclusive em script idempotente.
            migrationBuilder.Sql(@"EXEC(N'
CREATE OR ALTER PROCEDURE sp_Combos_BuscarPorDestino
    @Destino NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nome, Destino, Vagas, Categoria, Valor
      FROM Combos
     WHERE Vagas > 0
       AND (@Destino IS NULL OR Destino LIKE N''%'' + @Destino + N''%'')
     ORDER BY Nome;
END');");

            // ---------- Compras.ComboId passa a ser obrigatório ----------
            // Compras antigas sem combo não são ligadas a um combo qualquer: se existirem,
            // a migration para de propósito, para o grupo decidir o que fazer com elas.
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM Compras WHERE ComboId IS NULL)
    THROW 51000, 'Existem compras sem combo. Revise esses registros antes de aplicar CatalogoCombos.', 1;");

            // O SQL Server não deixa alterar uma coluna que tem índice e chave estrangeira
            // apontando para ela. Por isso: tira os dois, altera a coluna e põe os dois de volta.
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Combos_ComboId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_ComboId",
                table: "Compras");

            migrationBuilder.Sql("ALTER TABLE [Compras] ALTER COLUMN [ComboId] int NOT NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ComboId",
                table: "Compras",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Combos_ComboId",
                table: "Compras",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_Combos_ComboId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_ComboId",
                table: "Compras");

            migrationBuilder.Sql("ALTER TABLE [Compras] ALTER COLUMN [ComboId] int NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ComboId",
                table: "Compras",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_Combos_ComboId",
                table: "Compras",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_Combos_BuscarPorDestino;");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Combos_Vagas",
                table: "Combos");

            migrationBuilder.DropColumn(name: "Nome", table: "Combos");
            migrationBuilder.DropColumn(name: "Destino", table: "Combos");
            migrationBuilder.DropColumn(name: "Vagas", table: "Combos");
            migrationBuilder.DropColumn(name: "Categoria", table: "Combos");
        }
    }
}
