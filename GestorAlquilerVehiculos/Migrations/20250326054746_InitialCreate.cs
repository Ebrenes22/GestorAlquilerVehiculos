using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorAlquilerVehiculos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientesReserva",
                columns: table => new
                {
                    ClienteReservaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesReserva", x => x.ClienteReservaID);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    VehiculoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Placa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioPorDia = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ImagenURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.VehiculoID);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    ReporteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    TipoReporte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.ReporteID);
                    table.ForeignKey(
                        name: "FK_Reportes_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantenimientos",
                columns: table => new
                {
                    MantenimientoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehiculoID = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaMantenimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantenimientos", x => x.MantenimientoID);
                    table.ForeignKey(
                        name: "FK_Mantenimientos_Vehiculos_VehiculoID",
                        column: x => x.VehiculoID,
                        principalTable: "Vehiculos",
                        principalColumn: "VehiculoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    ReservaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: true),
                    ClienteReservaID = table.Column<int>(type: "int", nullable: true),
                    VehiculoID = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CostoTotal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.ReservaID);
                    table.CheckConstraint("CK_Reserva_ClienteOUsuario", "(UsuarioID IS NOT NULL AND ClienteReservaID IS NULL) OR (UsuarioID IS NULL AND ClienteReservaID IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Reservas_ClientesReserva_ClienteReservaID",
                        column: x => x.ClienteReservaID,
                        principalTable: "ClientesReserva",
                        principalColumn: "ClienteReservaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Vehiculos_VehiculoID",
                        column: x => x.VehiculoID,
                        principalTable: "Vehiculos",
                        principalColumn: "VehiculoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargosAdicionales",
                columns: table => new
                {
                    CargoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaID = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargosAdicionales", x => x.CargoID);
                    table.ForeignKey(
                        name: "FK_CargosAdicionales_Reservas_ReservaID",
                        column: x => x.ReservaID,
                        principalTable: "Reservas",
                        principalColumn: "ReservaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntregasDevoluciones",
                columns: table => new
                {
                    EntregaDevolucionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaID = table.Column<int>(type: "int", nullable: false),
                    EstadoInicial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoFinal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CargosAdicionales = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasDevoluciones", x => x.EntregaDevolucionID);
                    table.ForeignKey(
                        name: "FK_EntregasDevoluciones_Reservas_ReservaID",
                        column: x => x.ReservaID,
                        principalTable: "Reservas",
                        principalColumn: "ReservaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    NotificacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaID = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.NotificacionID);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Reservas_ReservaID",
                        column: x => x.ReservaID,
                        principalTable: "Reservas",
                        principalColumn: "ReservaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargosAdicionales_ReservaID",
                table: "CargosAdicionales",
                column: "ReservaID");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasDevoluciones_ReservaID",
                table: "EntregasDevoluciones",
                column: "ReservaID");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimientos_VehiculoID",
                table: "Mantenimientos",
                column: "VehiculoID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_ReservaID",
                table: "Notificaciones",
                column: "ReservaID");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_UsuarioID",
                table: "Reportes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteReservaID",
                table: "Reservas",
                column: "ClienteReservaID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioID",
                table: "Reservas",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_VehiculoID",
                table: "Reservas",
                column: "VehiculoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargosAdicionales");

            migrationBuilder.DropTable(
                name: "EntregasDevoluciones");

            migrationBuilder.DropTable(
                name: "Mantenimientos");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "ClientesReserva");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Vehiculos");
        }
    }
}
