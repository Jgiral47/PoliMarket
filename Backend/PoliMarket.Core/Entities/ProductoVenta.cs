using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad de relación Venta-Producto según diagrama UML
    /// Representa los productos incluidos en una venta específica
    /// </summary>
    public class VentaProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdVenta { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Relaciones
        [ForeignKey("IdVenta")]
        public virtual Venta Venta { get; set; } = null!;

        [ForeignKey("IdProducto")]
        public virtual Producto Producto { get; set; } = null!;

        // Método para calcular subtotal automáticamente
        public decimal CalcularSubtotal()
        {
            Subtotal = PrecioUnitario * Cantidad;
            return Subtotal;
        }

        // Constructor que calcula automáticamente el subtotal
        public VentaProducto()
        {
            // Constructor vacío para EF
        }

        public VentaProducto(int idVenta, int idProducto, int cantidad, decimal precioUnitario)
        {
            IdVenta = idVenta;
            IdProducto = idProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            CalcularSubtotal();
        }
    }
}