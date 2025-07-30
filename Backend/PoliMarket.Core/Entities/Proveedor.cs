using System.ComponentModel.DataAnnotations;

namespace PoliMarket.Core.Entities
{
    public class Proveedor : BaseEntity
    {
        public int IdProveedor { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string Correo { get; set; } = string.Empty;

        // Relaciones
        public virtual ICollection<ProveedorProducto> ProductosProveedor { get; set; } = new List<ProveedorProducto>();

        // Método reutilizable del componente
        public List<Producto> ListarProductosSuministrados()
        {
            return ProductosProveedor.Where(pp => pp.Activo)
                                   .Select(pp => pp.Producto)
                                   .ToList();
        }
    }
}