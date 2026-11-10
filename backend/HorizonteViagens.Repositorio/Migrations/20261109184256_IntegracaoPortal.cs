using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonteViagens.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class IntegracaoPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras");

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadaEm",
                table: "Compras",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Destino",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiraEm",
                table: "Compras",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Noites",
                table: "Compras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NomePacote",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PagamentoId",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PagamentoUrl",
                table: "Compras",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Partida",
                table: "Compras",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferenciaId",
                table: "Compras",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quartos",
                table: "Compras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RequisicaoId",
                table: "Compras",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Compras",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Compras",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Viajantes",
                table: "Compras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Combos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiariaQuarto",
                table: "Combos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Evento",
                table: "Combos",
                type: "nvarchar(140)",
                maxLength: 140,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Combos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Inclusoes",
                table: "Combos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Noites",
                table: "Combos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Origem",
                table: "Combos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Partida",
                table: "Combos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrecoConfirmado",
                table: "Combos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Publicado",
                table: "Combos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorExperiencias",
                table: "Combos",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcessoAte",
                table: "Clientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcessoAte",
                table: "Administradores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmitidoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favoritos",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ComboId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoritos", x => new { x.ClienteId, x.ComboId });
                    table.ForeignKey(
                        name: "FK_Favoritos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favoritos_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Administradores",
                keyColumn: "Id",
                keyValue: 1,
                column: "AcessoAte",
                value: null);

            // ALTERAÇÃO PIM: preserva compras antigas sem transformá-las em pagamento confirmado.
            // NEWID por linha evita colisão no novo índice de idempotência.
            migrationBuilder.Sql(@"
UPDATE Combos SET Noites=1, Imagem='beach', Origem=N'São Paulo' WHERE Noites=0;
UPDATE c SET RequisicaoId=NEWID(), Viajantes=1, Quartos=1, Noites=1,
 Status=N'Legado - revisar', NomePacote=b.Nome, Destino=b.Destino
 FROM Compras c JOIN Combos b ON b.Id=c.ComboId
 WHERE c.RequisicaoId='00000000-0000-0000-0000-000000000000';");
            // ALTERAÇÃO PIM: a procedure retorna os novos metadados e respeita a publicação.
            // ALTERAÇÃO PIM: EXEC isola o CREATE no primeiro comando do lote, inclusive em script idempotente.
            migrationBuilder.Sql(@"EXEC(N'
CREATE OR ALTER PROCEDURE sp_Combos_BuscarPorDestino @Destino NVARCHAR(100)=NULL
AS BEGIN SET NOCOUNT ON;
 SELECT * FROM Combos WHERE Publicado=1 AND Vagas>0
 AND (@Destino IS NULL OR Destino LIKE N''%'' + @Destino + N''%'') ORDER BY Nome;
END');");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId_RequisicaoId",
                table: "Compras",
                columns: new[] { "ClienteId", "RequisicaoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_PagamentoId",
                table: "Compras",
                column: "PagamentoId",
                unique: true,
                filter: "[PagamentoId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Compras_Viajantes",
                table: "Compras",
                sql: "[Viajantes] BETWEEN 1 AND 6");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Combos_Precificacao",
                table: "Combos",
                sql: "[Noites] BETWEEN 1 AND 60 AND [DiariaQuarto] >= 0 AND [ValorExperiencias] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_CompraId",
                table: "Documentos",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_ComboId",
                table: "Favoritos",
                column: "ComboId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ALTERAÇÃO PIM: restaura a procedure antes de retirar as colunas novas.
            // ALTERAÇÃO PIM: EXEC isola o CREATE no primeiro comando do lote, inclusive em script idempotente.
            migrationBuilder.Sql(@"EXEC(N'CREATE OR ALTER PROCEDURE sp_Combos_BuscarPorDestino @Destino NVARCHAR(100)=NULL AS BEGIN SET NOCOUNT ON; SELECT Id,Nome,Destino,Vagas,Categoria,Valor FROM Combos WHERE Vagas>0 AND (@Destino IS NULL OR Destino LIKE N''%'' + @Destino + N''%'') ORDER BY Nome; END');");
            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Favoritos");

            migrationBuilder.DropIndex(
                name: "IX_Compras_ClienteId_RequisicaoId",
                table: "Compras");

            migrationBuilder.DropIndex(
                name: "IX_Compras_PagamentoId",
                table: "Compras");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Compras_Viajantes",
                table: "Compras");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Combos_Precificacao",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "CriadaEm",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Destino",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "ExpiraEm",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Noites",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "NomePacote",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "PagamentoId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "PagamentoUrl",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Partida",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "PreferenciaId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Quartos",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "RequisicaoId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Viajantes",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "DiariaQuarto",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Evento",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Inclusoes",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Noites",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Origem",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Partida",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "PrecoConfirmado",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "Publicado",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "ValorExperiencias",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "AcessoAte",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "AcessoAte",
                table: "Administradores");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                column: "ClienteId");
        }
    }
}
