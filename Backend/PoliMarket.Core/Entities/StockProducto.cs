namespace PoliMarket.Core.Entities
{
    public class StockProducto : BaseEntity
    {
        public int IdProducto { get; set; }
        public int CantidadDisponible { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        public virtual Producto Producto { get; set; } = null!;

        // Métodos reutilizables del componente
        public void ActualizarStock(int nuevaCantidad)
        {
            CantidadDisponible = nuevaCantidad;
            FechaActualizacion = DateTime.UtcNow;
        }

        public int ObtenerStock()
        {
            return CantidadDisponible;
        }
    }
}