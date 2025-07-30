using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PoliMarket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    TipoPersona = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CodigoCliente = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdVendedor = table.Column<int>(type: "int", nullable: true),
                    EstaAutorizado = table.Column<bool>(type: "bit", nullable: true),
                    FechaAutorizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StockDisponible = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Autorizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAutorizacion = table.Column<int>(type: "int", nullable: false),
                    IdVendedor = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaVigencia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EsVigente = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autorizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Autorizaciones_Personas_IdVendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdVendedor = table.Column<int>(type: "int", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumeroFactura = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaFactura = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Personas_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_Personas_IdVendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProveedorProductos_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorProductos_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entregas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEntrega = table.Column<int>(type: "int", nullable: false),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoEntrega = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entregas_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentaProductos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVenta = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaProductos_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VentaProductos_Ventas_IdVenta",
                        column: x => x.IdVenta,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "Activo", "Apellido", "EstaAutorizado", "FechaAutorizacion", "FechaCreacion", "IdPersona", "IdVendedor", "Identificacion", "Nombre", "TipoPersona" },
                values: new object[,]
                {
                    { 1, true, "Pérez", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3001, 4001, "12345678", "Juan Carlos", "Vendedor" },
                    { 2, true, "González", true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3002, 4002, "87654321", "María Elena", "Vendedor" },
                    { 3, true, "Rodríguez", false, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3003, 4003, "11223344", "Carlos Alberto", "Vendedor" }
                });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "Activo", "Apellido", "CodigoCliente", "FechaCreacion", "FechaRegistro", "IdPersona", "Identificacion", "Nombre", "TipoPersona" },
                values: new object[,]
                {
                    { 4, true, "Martínez", 5001, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3004, "99887766", "Ana Sofía", "Cliente" },
                    { 5, true, "Jiménez", 5002, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3005, "55443322", "Roberto", "Cliente" },
                    { 6, true, "Fernández", 5003, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3006, "66778899", "Lucía", "Cliente" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "IdProducto", "Nombre", "Precio", "StockDisponible" },
                values: new object[,]
                {
                    { 1, true, "Laptop para uso profesional", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2001, "Laptop Dell Inspiron", 2500000m, 15 },
                    { 2, true, "Mouse ergonómico inalámbrico", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2002, "Mouse Inalámbrico", 85000m, 50 },
                    { 3, true, "Teclado mecánico RGB", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2003, "Teclado Mecánico", 320000m, 25 },
                    { 4, true, "Monitor Full HD IPS", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2004, "Monitor 24 pulgadas", 890000m, 8 },
                    { 5, true, "Disco sólido de alta velocidad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2005, "Disco SSD 1TB", 450000m, 30 }
                });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Activo", "Correo", "IdProveedor", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "contacto@techsolutions.com", 1001, "Proveedor Tech Solutions", "555-0001" },
                    { 2, true, "ventas@disnacional.com", 1002, "Distribuidora Nacional", "555-0002" }
                });

            migrationBuilder.InsertData(
                table: "Autorizaciones",
                columns: new[] { "Id", "EsVigente", "FechaCreacion", "FechaVigencia", "IdAutorizacion", "IdVendedor", "Tipo" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6001, 1, "Venta" },
                    { 2, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6002, 2, "Venta" }
                });

            migrationBuilder.InsertData(
                table: "ProveedorProductos",
                columns: new[] { "Id", "Activo", "FechaInicio", "IdProducto", "IdProveedor" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 3, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2 },
                    { 4, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 2 },
                    { 5, true, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autorizaciones_IdAutorizacion",
                table: "Autorizaciones",
                column: "IdAutorizacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autorizaciones_IdVendedor",
                table: "Autorizaciones",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_IdEntrega",
                table: "Entregas",
                column: "IdEntrega",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_IdVenta",
                table: "Entregas",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CodigoCliente",
                table: "Personas",
                column: "CodigoCliente",
                unique: true,
                filter: "[CodigoCliente] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_Identificacion",
                table: "Personas",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_IdVendedor",
                table: "Personas",
                column: "IdVendedor",
                unique: true,
                filter: "[IdVendedor] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdProducto",
                table: "Productos",
                column: "IdProducto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_IdProveedor",
                table: "Proveedores",
                column: "IdProveedor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_IdProducto",
                table: "ProveedorProductos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_IdProveedor",
                table: "ProveedorProductos",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_VentaProductos_IdProducto",
                table: "VentaProductos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_VentaProductos_IdVenta",
                table: "VentaProductos",
                column: "IdVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdCliente",
                table: "Ventas",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdVendedor",
                table: "Ventas",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdVenta",
                table: "Ventas",
                column: "IdVenta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autorizaciones");

            migrationBuilder.DropTable(
                name: "Entregas");

            migrationBuilder.DropTable(
                name: "ProveedorProductos");

            migrationBuilder.DropTable(
                name: "VentaProductos");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
