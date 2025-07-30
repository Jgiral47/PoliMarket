using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    public class Producto : BaseEntity
    {
        public int IdProducto { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public double Precio { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        // Relaciones
        public virtual StockProducto? Stock { get; set; }
        public virtual ICollection<ProductoVenta> ProductosVenta { get; set; } = new List<ProductoVenta>();
        public virtual ICollection<ProveedorProducto> ProveedoresProducto { get; set; } = new List<ProveedorProducto>();

        // Método reutilizable del componente
        public bool EstaDisponible(int cantidad)
        {
            return Stock?.CantidadDisponible >= cantidad;
        }
    }
}