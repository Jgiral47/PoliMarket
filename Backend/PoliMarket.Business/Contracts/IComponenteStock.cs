
using PoliMarket.Core.Entities;
using PoliMarket.Business.DTOs;

namespace PoliMarket.Business.Contracts
{
    // <summary>    
    /// RF5: Consultar disponibilidad de stock de productos
    /// </summary>
    public interface IComponenteStock
    {
        // RF5: Consultar disponibilidad
        Task<DisponibilidadStockDto> ConsultarDisponibilidadAsync(int productoId, int cantidad);
        

        // Métodos auxiliares
        Task<int> ObtenerStockAsync(int productoId);
        Task<bool> ReabastecerStockAsync(int productoId, int cantidad);
        Task<IEnumerable<Producto>> ObtenerProductosBajoStockAsync(int stockMinimo = 5);
        Task<IEnumerable<Producto>> ObtenerProductosDisponiblesAsync();
    }
}
