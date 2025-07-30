using Microsoft.EntityFrameworkCore;
using PoliMarket.Core.Entities;

namespace PoliMarket.DataAccess.Context
{
    /// <summary>
    /// Contexto principal de Entity Framework para PoliMarket
    /// Configuración de todas las entidades y relaciones del diagrama UML
    /// </summary>
    public class PoliMarketDbContext : DbContext
    {
        public PoliMarketDbContext(DbContextOptions<PoliMarketDbContext> options) : base(options)
        {
        }

        // DbSets para las entidades principales
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaProducto> VentaProductos { get; set; }
        public DbSet<Autorizacion> Autorizaciones { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProveedorProducto> ProveedorProductos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de herencia TPT (Table Per Type) para Persona
            ConfigurarHerenciaPersona(modelBuilder);

            // Configuración de relaciones según diagrama UML
            ConfigurarRelaciones(modelBuilder);

            // Configuración de propiedades y restricciones
            ConfigurarPropiedades(modelBuilder);

            // Datos semilla para pruebas
            ConfigurarDatosSemilla(modelBuilder);
        }

        /// <summary>
        /// Configura la herencia TPT para Persona, Vendedor y Cliente
        /// </summary>
        private void ConfigurarHerenciaPersona(ModelBuilder modelBuilder)
        {
            // Configurar tabla base Persona como abstracta
            modelBuilder.Entity<Persona>()
                .ToTable("Personas")
                .HasDiscriminator<string>("TipoPersona")
                .HasValue<Vendedor>("Vendedor")
                .HasValue<Cliente>("Cliente");

            // Configurar propiedades comunes de Persona
            modelBuilder.Entity<Persona>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Persona>()
                .HasIndex(p => p.Identificacion)
                .IsUnique();

            modelBuilder.Entity<Persona>()
                .Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Apellido)
                .IsRequired()
                .HasMaxLength(100);
        }

        /// <summary>
        /// Configura las relaciones entre entidades según el diagrama UML
        /// </summary>
        private void ConfigurarRelaciones(ModelBuilder modelBuilder)
        {
            // Relación Vendedor 1:N Autorizacion
            modelBuilder.Entity<Autorizacion>()
                .HasOne(a => a.Vendedor)
                .WithMany(v => v.Autorizaciones)
                .HasForeignKey(a => a.IdVendedor)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Vendedor 1:N Venta
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Vendedor)
                .WithMany(ve => ve.Ventas)
                .HasForeignKey(v => v.IdVendedor)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Cliente 1:N Venta
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Venta 1:N Entrega
            modelBuilder.Entity<Entrega>()
                .HasOne(e => e.Venta)
                .WithMany(v => v.Entregas)
                .HasForeignKey(e => e.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación N:M Venta-Producto a través de VentaProducto
            modelBuilder.Entity<VentaProducto>()
                .HasKey(vp => vp.Id);

            modelBuilder.Entity<VentaProducto>()
                .HasOne(vp => vp.Venta)
                .WithMany(v => v.VentaProductos)
                .HasForeignKey(vp => vp.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VentaProducto>()
                .HasOne(vp => vp.Producto)
                .WithMany(p => p.VentaProductos)
                .HasForeignKey(vp => vp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación N:M Proveedor-Producto a través de ProveedorProducto
            modelBuilder.Entity<ProveedorProducto>()
                .HasKey(pp => pp.Id);

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(p => p.ProveedorProductos)
                .HasForeignKey(pp => pp.IdProveedor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProveedorProductos)
                .HasForeignKey(pp => pp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Configura propiedades específicas y restricciones
        /// </summary>
        private void ConfigurarPropiedades(ModelBuilder modelBuilder)
        {
            // Configuración de Producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.IdProducto)
                .IsUnique();

            // Configuración de Venta
            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Venta>()
                .HasIndex(v => v.IdVenta)
                .IsUnique();

            // Configuración de VentaProducto
            modelBuilder.Entity<VentaProducto>()
                .Property(vp => vp.PrecioUnitario)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            modelBuilder.Entity<VentaProducto>()
                .Property(vp => vp.Subtotal)
                .HasColumnType("decimal(18,2)");

            // Índices únicos para identificadores de negocio
            modelBuilder.Entity<Vendedor>()
                .HasIndex(v => v.IdVendedor)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CodigoCliente)
                .IsUnique();

            modelBuilder.Entity<Autorizacion>()
                .HasIndex(a => a.IdAutorizacion)
                .IsUnique();

            modelBuilder.Entity<Entrega>()
                .HasIndex(e => e.IdEntrega)
                .IsUnique();

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.IdProveedor)
                .IsUnique();
        }

        /// <summary>
        /// Configura datos semilla para desarrollo y pruebas
        /// IMPORTANTE: Usar fechas fijas para evitar problemas con migrations
        /// </summary>
        private void ConfigurarDatosSemilla(ModelBuilder modelBuilder)
        {
            // Fechas fijas para evitar problemas con migrations
            var fechaAutorizacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var fechaVigencia = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var fechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Proveedores semilla
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor
                {
                    Id = 1,
                    IdProveedor = 1001,
                    Nombre = "Proveedor Tech Solutions",
                    Telefono = "555-0001",
                    Correo = "contacto@techsolutions.com",
                    Activo = true
                },
                new Proveedor
                {
                    Id = 2,
                    IdProveedor = 1002,
                    Nombre = "Distribuidora Nacional",
                    Telefono = "555-0002",
                    Correo = "ventas@disnacional.com",
                    Activo = true
                }
            );

            // Productos semilla
            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    Id = 1,
                    IdProducto = 2001,
                    Nombre = "Laptop Dell Inspiron",
                    Precio = 2500000,
                    Descripcion = "Laptop para uso profesional",
                    StockDisponible = 15,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Producto
                {
                    Id = 2,
                    IdProducto = 2002,
                    Nombre = "Mouse Inalámbrico",
                    Precio = 85000,
                    Descripcion = "Mouse ergonómico inalámbrico",
                    StockDisponible = 50,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Producto
                {
                    Id = 3,
                    IdProducto = 2003,
                    Nombre = "Teclado Mecánico",
                    Precio = 320000,
                    Descripcion = "Teclado mecánico RGB",
                    StockDisponible = 25,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Producto
                {
                    Id = 4,
                    IdProducto = 2004,
                    Nombre = "Monitor 24 pulgadas",
                    Precio = 890000,
                    Descripcion = "Monitor Full HD IPS",
                    StockDisponible = 8,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Producto
                {
                    Id = 5,
                    IdProducto = 2005,
                    Nombre = "Disco SSD 1TB",
                    Precio = 450000,
                    Descripcion = "Disco sólido de alta velocidad",
                    StockDisponible = 30,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                }
            );

            // Vendedores semilla
            modelBuilder.Entity<Vendedor>().HasData(
                new Vendedor
                {
                    Id = 1,
                    IdPersona = 3001,
                    IdVendedor = 4001,
                    Nombre = "Juan Carlos",
                    Apellido = "Pérez",
                    Identificacion = "12345678",
                    EstaAutorizado = true,
                    FechaAutorizacion = fechaAutorizacion,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Vendedor
                {
                    Id = 2,
                    IdPersona = 3002,
                    IdVendedor = 4002,
                    Nombre = "María Elena",
                    Apellido = "González",
                    Identificacion = "87654321",
                    EstaAutorizado = true,
                    FechaAutorizacion = fechaAutorizacion,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Vendedor
                {
                    Id = 3,
                    IdPersona = 3003,
                    IdVendedor = 4003,
                    Nombre = "Carlos Alberto",
                    Apellido = "Rodríguez",
                    Identificacion = "11223344",
                    EstaAutorizado = false,
                    Activo = true,
                    FechaCreacion = fechaCreacion
                }
            );

            // Clientes semilla
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 4,
                    IdPersona = 3004,
                    CodigoCliente = 5001,
                    Nombre = "Ana Sofía",
                    Apellido = "Martínez",
                    Identificacion = "99887766",
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Cliente
                {
                    Id = 5,
                    IdPersona = 3005,
                    CodigoCliente = 5002,
                    Nombre = "Roberto",
                    Apellido = "Jiménez",
                    Identificacion = "55443322",
                    Activo = true,
                    FechaCreacion = fechaCreacion
                },
                new Cliente
                {
                    Id = 6,
                    IdPersona = 3006,
                    CodigoCliente = 5003,
                    Nombre = "Lucía",
                    Apellido = "Fernández",
                    Identificacion = "66778899",
                    Activo = true,
                    FechaCreacion = fechaCreacion
                }
            );

            // Autorizaciones semilla
            modelBuilder.Entity<Autorizacion>().HasData(
                new Autorizacion
                {
                    Id = 1,
                    IdAutorizacion = 6001,
                    IdVendedor = 1,
                    Tipo = "Venta",
                    FechaVigencia = fechaVigencia,
                    EsVigente = true,
                    FechaCreacion = fechaCreacion
                },
                new Autorizacion
                {
                    Id = 2,
                    IdAutorizacion = 6002,
                    IdVendedor = 2,
                    Tipo = "Venta",
                    FechaVigencia = fechaVigencia,
                    EsVigente = true,
                    FechaCreacion = fechaCreacion
                }
            );

            // Relaciones Proveedor-Producto semilla
            modelBuilder.Entity<ProveedorProducto>().HasData(
                new ProveedorProducto
                {
                    Id = 1,
                    IdProveedor = 1,
                    IdProducto = 1,
                    Activo = true,
                    FechaInicio = fechaCreacion
                },
                new ProveedorProducto
                {
                    Id = 2,
                    IdProveedor = 1,
                    IdProducto = 4,
                    Activo = true,
                    FechaInicio = fechaCreacion
                },
                new ProveedorProducto
                {
                    Id = 3,
                    IdProveedor = 2,
                    IdProducto = 2,
                    Activo = true,
                    FechaInicio = fechaCreacion
                },
                new ProveedorProducto
                {
                    Id = 4,
                    IdProveedor = 2,
                    IdProducto = 3,
                    Activo = true,
                    FechaInicio = fechaCreacion
                },
                new ProveedorProducto
                {
                    Id = 5,
                    IdProveedor = 2,
                    IdProducto = 5,
                    Activo = true,
                    FechaInicio = fechaCreacion
                }
            );
        }
    }
}