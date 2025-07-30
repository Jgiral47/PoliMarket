using PoliMarket.Core.Entities;

namespace PoliMarket.Business.Interfaces
{
    public interface IStockService : IBaseService<StockProducto>
    {
        // RF6: Gestionar y actualizar stock cuando se realiza una venta o entrega
        Task<bool> ActualizarStockAsync(int productoId, int cantidad);
        Task<int> ObtenerStockAsync(int productoId);
        Task<bool> ValidarDisponibilidadAsync(List<(int productoId, int cantidad)> productos);
    }
}