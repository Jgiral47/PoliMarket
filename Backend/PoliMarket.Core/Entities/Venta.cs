using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad Venta según diagrama UML
    /// RF2: Registrar ventas de productos a clientes
    /// RF3: Calcular totales, generar factura y agregar productos a una venta
    /// </summary>
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdVenta { get; set; }

        [Required]
        public int IdVendedor { get; set; }

        [Required]
        public int IdCliente { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Completada, Cancelada

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0;

        public string? NumeroFactura { get; set; }

        public DateTime? FechaFactura { get; set; }

        // Relaciones según diagrama UML
        [ForeignKey("IdVendedor")]
        public virtual Vendedor Vendedor { get; set; } = null!;

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; } = null!;

        public virtual ICollection<VentaProducto> VentaProductos { get; set; } = new List<VentaProducto>();
        public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

        // Métodos de negocio para RF2 y RF3
        public decimal CalcularTotal()
        {
            Total = VentaProductos.Sum(vp => vp.Subtotal);
            return Total;
        }

        public string GenerarFactura()
        {
            if (string.IsNullOrEmpty(NumeroFactura))
            {
                NumeroFactura = $"FAC-{IdVenta}-{DateTime.UtcNow:yyyyMMddHHmmss}";
                FechaFactura = DateTime.UtcNow;
            }
            return NumeroFactura;
        }

        public bool AgregarProducto(int idProducto, int cantidad, decimal precio)
        {
            var ventaProducto = VentaProductos.FirstOrDefault(vp => vp.IdProducto == idProducto);

            if (ventaProducto != null)
            {
                // Actualizar cantidad si ya existe
                ventaProducto.Cantidad += cantidad;
                ventaProducto.CalcularSubtotal();
            }
            else
            {
                // Agregar nuevo producto
                VentaProductos.Add(new VentaProducto
                {
                    IdVenta = this.Id,
                    IdProducto = idProducto,
                    Cantidad = cantidad,
                    PrecioUnitario = precio
                });
            }

            CalcularTotal();
            return true;
        }

        public void CompletarVenta()
        {
            Estado = "Completada";
            GenerarFactura();
        }

        public void CancelarVenta()
        {
            Estado = "Cancelada";
        }
    }
}