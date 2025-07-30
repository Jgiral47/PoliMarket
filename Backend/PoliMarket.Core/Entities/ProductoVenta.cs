namespace PoliMarket.Core.Entities
{
    public class ProductoVenta : BaseEntity
    {
        public int IdProducto { get; set; }
        public int IdVenta { get; set; }
        public int Cantidad { get; set; }
        public double PrecioUnitario { get; set; }

        // Relaciones
        public virtual Producto Producto { get; set; } = null!;
        public virtual Venta Venta { get; set; } = null!;

        // Método reutilizable del componente
        public double CalcularSubtotal()
        {
            return Cantidad * PrecioUnitario;
        }
    }
}