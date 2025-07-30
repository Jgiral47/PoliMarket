namespace PoliMarket.Core.Entities
{
    public class Venta : BaseEntity
    {
        public int IdVenta { get; set; }
        public int IdVendedor { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;
        public string Estado { get; set; } = "PENDIENTE";

        // Relaciones
        public virtual Vendedor Vendedor { get; set; } = null!;
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual ICollection<ProductoVenta> ProductosVenta { get; set; } = new List<ProductoVenta>();
        public virtual OrdenEntrega? OrdenEntrega { get; set; }

        // Métodos reutilizables del componente
        public double CalcularTotal()
        {
            return ProductosVenta.Sum(pv => pv.CalcularSubtotal());
        }

        public string GenerarFactura()
        {
            return $"Factura #{IdVenta} - Cliente: {Cliente?.ObtenerNombreCompleto()} - Total: {CalcularTotal():C}";
        }

        public void AgregarProducto(int idProducto, int cantidad, double precioUnitario)
        {
            ProductosVenta.Add(new ProductoVenta
            {
                IdProducto = idProducto,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario
            });
        }
    }
}