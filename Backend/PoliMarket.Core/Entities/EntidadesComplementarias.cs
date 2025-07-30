using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliMarket.Core.Entities
{
    /// <summary>
    /// Entidad Autorizacion según diagrama UML
    /// RF1: Autorizar vendedores para operar dentro del sistema
    /// </summary>
    public class Autorizacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdAutorizacion { get; set; }

        [Required]
        public int IdVendedor { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty; // "Venta", "Administracion", etc.

        public DateTime FechaVigencia { get; set; }

        public bool EsVigente { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación
        [ForeignKey("IdVendedor")]
        public virtual Vendedor Vendedor { get; set; } = null!;

        // Método de negocio
        public bool ObtenerEstadoAutorizacion()
        {
            return EsVigente && FechaVigencia > DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Entidad Entrega según diagrama UML
    /// RF8: Registrar entregas realizadas y actualizar el estado de las órdenes
    /// </summary>
    public class Entrega
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdEntrega { get; set; }

        [Required]
        public int IdVenta { get; set; }

        public DateTime FechaEntrega { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string EstadoEntrega { get; set; } = "Pendiente"; // Pendiente, EnTransito, Entregado

        // Relación
        [ForeignKey("IdVenta")]
        public virtual Venta Venta { get; set; } = null!;

        // Método de negocio
        public string GenerarResumen()
        {
            return $"Entrega #{IdEntrega} - Estado: {EstadoEntrega} - Fecha: {FechaEntrega:dd/MM/yyyy}";
        }

        public void ActualizarEstado(string nuevoEstado)
        {
            EstadoEntrega = nuevoEstado;
            if (nuevoEstado == "Entregado")
            {
                FechaEntrega = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Entidad Proveedor según diagrama UML
    /// RF7: Consultar qué productos suministra cada proveedor
    /// </summary>
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        public bool Activo { get; set; } = true;

        // Relaciones
        public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();

        // Métodos de negocio para RF7
        public List<Producto> ListarProductosSuministrados()
        {
            return ProveedorProductos
                .Where(pp => pp.Activo)
                .Select(pp => pp.Producto)
                .ToList();
        }

        public string ObtenerInfoSuministro(int idProducto)
        {
            var relacion = ProveedorProductos.FirstOrDefault(pp => pp.IdProducto == idProducto);
            return relacion != null
                ? $"Proveedor: {Nombre} - Producto suministrado desde: {relacion.FechaInicio:dd/MM/yyyy}"
                : "No suministra este producto";
        }
    }

    /// <summary>
    /// Entidad de relación Proveedor-Producto
    /// </summary>
    public class ProveedorProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [Required]
        public int IdProducto { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        // Relaciones
        [ForeignKey("IdProveedor")]
        public virtual Proveedor Proveedor { get; set; } = null!;

        [ForeignKey("IdProducto")]
        public virtual Producto Producto { get; set; } = null!;
    }
}