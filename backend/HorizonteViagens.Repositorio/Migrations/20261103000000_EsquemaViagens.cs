// ALTERAÇÃO PIM: Evolução do esquema, metadados EF e execução idempotente de SQL; arquivos gerados identificados.
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorizonteViagens.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class EsquemaViagens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------- Endereço do cliente ----------
            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "Clientes",
                type: "varchar(9)",
                unicode: false,
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Clientes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rua",
                table: "Clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // ---------- Tabelas sem dependência ----------
            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.Id);
                    table.CheckConstraint("CK_Combos_Valor", "[Valor] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Hoteis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Classificacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoteis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passeios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lugar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Horario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passeios", x => x.Id);
                    table.CheckConstraint("CK_Passeios_Capacidade", "[Capacidade] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Voos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CidadePartida = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoraPartida = table.Column<TimeOnly>(type: "time", nullable: false),
                    CidadeChegada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoraChegada = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voos", x => x.Id);
                });

            // ---------- Telefones ----------
            migrationBuilder.CreateTable(
                name: "TelefonesAdministradores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    AdministradorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonesAdministradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefonesAdministradores_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelefonesClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefonesClientes_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelefonesHoteis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonesHoteis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefonesHoteis_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ---------- Passagens, quartos e compras ----------
            migrationBuilder.CreateTable(
                name: "Passagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Classe = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Assento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataIda = table.Column<DateOnly>(type: "date", nullable: false),
                    DataVolta = table.Column<DateOnly>(type: "date", nullable: false),
                    VooId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passagens", x => x.Id);
                    table.CheckConstraint("CK_Passagens_Periodo", "[DataVolta] >= [DataIda]");
                    table.ForeignKey(
                        name: "FK_Passagens_Voos_VooId",
                        column: x => x.VooId,
                        principalTable: "Voos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservasQuartos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroQuarto = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DataCheckin = table.Column<DateOnly>(type: "date", nullable: false),
                    DataCheckout = table.Column<DateOnly>(type: "date", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasQuartos", x => x.Id);
                    table.CheckConstraint("CK_ReservasQuartos_Periodo", "[DataCheckout] > [DataCheckin]");
                    table.ForeignKey(
                        name: "FK_ReservasQuartos_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.CheckConstraint("CK_Compras_Valor", "[Valor] >= 0");
                    table.ForeignKey(
                        name: "FK_Compras_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compras_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ---------- Tabelas de ligação ----------
            migrationBuilder.CreateTable(
                name: "ItensCombos",
                columns: table => new
                {
                    QuartoId = table.Column<int>(type: "int", nullable: false),
                    PassagemId = table.Column<int>(type: "int", nullable: false),
                    PasseioId = table.Column<int>(type: "int", nullable: false),
                    ComboId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCombos", x => new { x.QuartoId, x.PassagemId, x.PasseioId, x.ComboId });
                    table.ForeignKey(
                        name: "FK_ItensCombos_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCombos_Passagens_PassagemId",
                        column: x => x.PassagemId,
                        principalTable: "Passagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCombos_Passeios_PasseioId",
                        column: x => x.PasseioId,
                        principalTable: "Passeios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensCombos_ReservasQuartos_QuartoId",
                        column: x => x.QuartoId,
                        principalTable: "ReservasQuartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonitoramentosCompras",
                columns: table => new
                {
                    AdministradorId = table.Column<int>(type: "int", nullable: false),
                    CompraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoramentosCompras", x => new { x.AdministradorId, x.CompraId });
                    table.ForeignKey(
                        name: "FK_MonitoramentosCompras_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonitoramentosCompras_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ---------- Índices das chaves estrangeiras ----------
            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ComboId",
                table: "Compras",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCombos_ComboId",
                table: "ItensCombos",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCombos_PassagemId",
                table: "ItensCombos",
                column: "PassagemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCombos_PasseioId",
                table: "ItensCombos",
                column: "PasseioId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoramentosCompras_CompraId",
                table: "MonitoramentosCompras",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Passagens_VooId",
                table: "Passagens",
                column: "VooId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasQuartos_HotelId",
                table: "ReservasQuartos",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_TelefonesAdministradores_AdministradorId",
                table: "TelefonesAdministradores",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_TelefonesClientes_ClienteId",
                table: "TelefonesClientes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TelefonesHoteis_HotelId",
                table: "TelefonesHoteis",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ordem inversa: primeiro quem depende, depois de quem se depende.
            migrationBuilder.DropTable(name: "ItensCombos");
            migrationBuilder.DropTable(name: "MonitoramentosCompras");
            migrationBuilder.DropTable(name: "TelefonesAdministradores");
            migrationBuilder.DropTable(name: "TelefonesClientes");
            migrationBuilder.DropTable(name: "TelefonesHoteis");
            migrationBuilder.DropTable(name: "Passagens");
            migrationBuilder.DropTable(name: "Passeios");
            migrationBuilder.DropTable(name: "ReservasQuartos");
            migrationBuilder.DropTable(name: "Compras");
            migrationBuilder.DropTable(name: "Voos");
            migrationBuilder.DropTable(name: "Hoteis");
            migrationBuilder.DropTable(name: "Combos");

            migrationBuilder.DropColumn(name: "Bairro", table: "Clientes");
            migrationBuilder.DropColumn(name: "CEP", table: "Clientes");
            migrationBuilder.DropColumn(name: "Cidade", table: "Clientes");
            migrationBuilder.DropColumn(name: "Rua", table: "Clientes");
        }
    }
}
