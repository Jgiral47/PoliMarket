using Microsoft.EntityFrameworkCore;
using PoliMarket.Core.Entities;

namespace PoliMarket.DataAccess.Context
{
    public class PoliMarketDbContext : DbContext
    {
        public PoliMarketDbContext(DbContextOptions<PoliMarketDbContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<ProductoVenta> ProductosVenta { get; set; }
        public DbSet<StockProducto> StockProductos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProveedorProducto> ProveedoresProductos { get; set; }
        public DbSet<OrdenEntrega> OrdenesEntrega { get; set; }
        public DbSet<HistoricoOrdenEntrega> HistoricoOrdenesEntrega { get; set; }
        public DbSet<VendedorAutorizacion> VendedoresAutorizacion { get; set; }
        public DbSet<Autorizacion> Autorizaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de herencia TPH (Table Per Hierarchy) para Persona
            modelBuilder.Entity<Persona>()
                .HasDiscriminator<string>("TipoPersona")
                .HasValue<Vendedor>("Vendedor")
                .HasValue<Cliente>("Cliente");

            // Configuración de relaciones SIN CASCADE DELETE para evitar ciclos
            ConfigurarRelacionesSinCascada(modelBuilder);

            // Configuración de propiedades
            ConfigurarPropiedades(modelBuilder);
        }

        private void ConfigurarRelacionesSinCascada(ModelBuilder modelBuilder)
        {
            // Relación Vendedor - VendedorAutorizacion
            modelBuilder.Entity<VendedorAutorizacion>()
                .HasKey(va => new { va.IdVendedor, va.IdAutorizacion });

            modelBuilder.Entity<VendedorAutorizacion>()
                .HasOne(va => va.Vendedor)
                .WithMany(v => v.Autorizaciones)
                .HasForeignKey(va => va.IdVendedor)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            modelBuilder.Entity<VendedorAutorizacion>()
                .HasOne(va => va.Autorizacion)
                .WithMany(a => a.VendedoresAutorizacion)
                .HasForeignKey(va => va.IdAutorizacion)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación Venta - Vendedor (SIN CASCADE)
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Vendedor)
                .WithMany(ve => ve.Ventas)
                .HasForeignKey(v => v.IdVendedor)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación Venta - Cliente (SIN CASCADE)
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.IdCliente)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación ProductoVenta (muchos a muchos entre Producto y Venta)
            modelBuilder.Entity<ProductoVenta>()
                .HasKey(pv => new { pv.IdProducto, pv.IdVenta });

            modelBuilder.Entity<ProductoVenta>()
                .HasOne(pv => pv.Producto)
                .WithMany(p => p.ProductosVenta)
                .HasForeignKey(pv => pv.IdProducto)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            modelBuilder.Entity<ProductoVenta>()
                .HasOne(pv => pv.Venta)
                .WithMany(v => v.ProductosVenta)
                .HasForeignKey(pv => pv.IdVenta)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación Producto - StockProducto (uno a uno)
            modelBuilder.Entity<StockProducto>()
                .HasOne(sp => sp.Producto)
                .WithOne(p => p.Stock)
                .HasForeignKey<StockProducto>(sp => sp.IdProducto)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación ProveedorProducto (muchos a muchos entre Proveedor y Producto)
            modelBuilder.Entity<ProveedorProducto>()
                .HasKey(pp => new { pp.IdProveedor, pp.IdProducto });

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Proveedor)
                .WithMany(p => p.ProductosProveedor)
                .HasForeignKey(pp => pp.IdProveedor)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Producto)
                .WithMany(p => p.ProveedoresProducto)
                .HasForeignKey(pp => pp.IdProducto)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación OrdenEntrega - Venta (uno a uno)
            modelBuilder.Entity<OrdenEntrega>()
                .HasOne(oe => oe.Venta)
                .WithOne(v => v.OrdenEntrega)
                .HasForeignKey<OrdenEntrega>(oe => oe.IdVenta)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada

            // Relación HistoricoOrdenEntrega - OrdenEntrega
            modelBuilder.Entity<HistoricoOrdenEntrega>()
                .HasOne(h => h.Orden)
                .WithMany(o => o.HistorialCambios)
                .HasForeignKey(h => h.IdOrden)
                .OnDelete(DeleteBehavior.NoAction); // Sin cascada
        }

        private void ConfigurarPropiedades(ModelBuilder modelBuilder)
        {
            // Configuración de decimales para precios
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductoVenta>()
                .Property(pv => pv.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            // Configuración de índices únicos
            modelBuilder.Entity<Persona>()
                .HasIndex(p => p.Identificacion)
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.IdProducto)
                .IsUnique();

            // Configurar longitudes de strings
            modelBuilder.Entity<Persona>()
                .Property(p => p.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Apellido)
                .HasMaxLength(100);

            modelBuilder.Entity<Persona>()
                .Property(p => p.Identificacion)
                .HasMaxLength(20);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Nombre)
                .HasMaxLength(100);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Descripcion)
                .HasMaxLength(500);
        }
    }
}