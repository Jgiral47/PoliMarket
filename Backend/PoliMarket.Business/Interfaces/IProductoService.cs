using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IProductoService : IBaseService<Producto>
    {
        // RF5: Consultar disponibilidad de stock de productos
        Task<bool> EstaDisponibleAsync(int productoId, int cantidad);
        Task<double> CalcularSubtotalAsync(int productoId, int cantidad);
    }
}