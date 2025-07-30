
using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Contracts
{
    /// <summary>
    /// ComponenteProducto - Operaciones sobre productos
    /// </summary>
    public interface IComponenteProducto
    {
        Task<IEnumerable<Producto>> ListarProductosAsync();
        Task<Producto?> ObtenerProductoPorIdAsync(int productoId);
        Task<bool> EstaDisponibleAsync(int productoId, int cantidad);
        Task<decimal> CalcularSubtotalAsync(int productoId, int cantidad);
        Task<IEnumerable<Producto>> BuscarProductosAsync(string termino);
    }
}
