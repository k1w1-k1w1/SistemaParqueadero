using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaParqueadero.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "parqueadero");

            migrationBuilder.CreateTable(
                name: "Usuarios1",
                schema: "parqueadero",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreCompleto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios1", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                schema: "parqueadero",
                columns: table => new
                {
                    VehiculoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Modelo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.VehiculoId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosParqueo",
                schema: "parqueadero",
                columns: table => new
                {
                    RegistroId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehiculoId = table.Column<int>(type: "integer", nullable: false),
                    TipoTarifa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaHoraIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioIngresoId = table.Column<int>(type: "integer", nullable: true),
                    ObservacionesIngreso = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaHoraSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioSalidaId = table.Column<int>(type: "integer", nullable: true),
                    HorasCalculadas = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    MontoCobrado = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    MetodoPago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ObservacionesSalida = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosParqueo", x => x.RegistroId);
                    table.ForeignKey(
                        name: "FK_RegistrosParqueo_Usuarios1_UsuarioIngresoId",
                        column: x => x.UsuarioIngresoId,
                        principalSchema: "parqueadero",
                        principalTable: "Usuarios1",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosParqueo_Usuarios1_UsuarioSalidaId",
                        column: x => x.UsuarioSalidaId,
                        principalSchema: "parqueadero",
                        principalTable: "Usuarios1",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosParqueo_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalSchema: "parqueadero",
                        principalTable: "Vehiculos",
                        principalColumn: "VehiculoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "parqueadero",
                table: "Usuarios1",
                columns: new[] { "UsuarioId", "Activo", "FechaCreacion", "NombreCompleto", "PasswordHash", "Rol", "Username" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 2, 22, 17, 51, 49, 385, DateTimeKind.Utc).AddTicks(4291), "Administrador del Sistema", "$2a$11$e3hgNTXLILMztMdzduJYxOWtifeFGS3b3P9DM4K0ZD/sxpYzgwfWW", "Administrador", "admin" },
                    { 2, true, new DateTime(2026, 2, 22, 17, 51, 49, 505, DateTimeKind.Utc).AddTicks(5650), "Operador Principal", "$2a$11$r2YPU32.gwJc0k9Zw8LuOOwr9IVhuWJODQegirX922/yDkcnj4EvW", "Operador", "operador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_Estado",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_FechaHoraIngreso",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "FechaHoraIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_FechaHoraSalida",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "FechaHoraSalida");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_UsuarioIngresoId",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "UsuarioIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_UsuarioSalidaId",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "UsuarioSalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParqueo_VehiculoId",
                schema: "parqueadero",
                table: "RegistrosParqueo",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios1_Username",
                schema: "parqueadero",
                table: "Usuarios1",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Placa",
                schema: "parqueadero",
                table: "Vehiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosParqueo",
                schema: "parqueadero");

            migrationBuilder.DropTable(
                name: "Usuarios1",
                schema: "parqueadero");

            migrationBuilder.DropTable(
                name: "Vehiculos",
                schema: "parqueadero");
        }
    }
}
