using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad Producto según diagrama UML
    /// RF5: Consultar disponibilidad de stock de productos
    /// RF6: Gestionar y actualizar stock
    /// </summary>
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        public int StockDisponible { get; set; } = 0;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        // Relaciones según diagrama UML
        public virtual ICollection<VentaProducto> VentaProductos { get; set; } = new List<VentaProducto>();
        public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();

        // Métodos de negocio para RF5 y RF6
        public bool EstaDisponible(int cantidad)
        {
            return Activo && StockDisponible >= cantidad;
        }

        public decimal CalcularSubtotal(int cantidad)
        {
            return Precio * cantidad;
        }

        public bool ActualizarStock(int cantidadVendida)
        {
            if (StockDisponible >= cantidadVendida)
            {
                StockDisponible -= cantidadVendida;
                return true;
            }
            return false;
        }

        public void reabastecer(int cantidad)
        {
            StockDisponible += cantidad;
        }
    }
}