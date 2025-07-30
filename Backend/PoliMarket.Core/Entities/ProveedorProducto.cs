namespace PoliMarket.Core.Entities
{
    public class ProveedorProducto : BaseEntity
    {
        public int IdProveedor { get; set; }
        public int IdProducto { get; set; }
        public DateTime FechaUltimaCompra { get; set; }
        public int CantidadUltimaCompra { get; set; }

        // Relaciones
        public virtual Proveedor Proveedor { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;

        // Método reutilizable del componente
        public string ObtenerInfoSuministro()
        {
            return $"Proveedor: {Proveedor?.Nombre} - Última compra: {FechaUltimaCompra:dd/MM/yyyy} - Cantidad: {CantidadUltimaCompra}";
        }
    }
}