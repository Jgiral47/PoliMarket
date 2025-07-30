using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class StockService : BaseService<StockProducto>, IStockService
    {
        public StockService(IGenericRepository<StockProducto> repository) : base(repository)
        {
        }

        // RF6: Gestionar y actualizar stock cuando se realiza una venta o entrega
        public async Task<bool> ActualizarStockAsync(int productoId, int cantidad)
        {
            try
            {
                var stocks = await _repository.GetAllAsync(
                    filter: s => s.IdProducto == productoId
                );

                var stock = stocks.FirstOrDefault();
                if (stock == null)
                {
                    // Crear nuevo stock si no existe
                    stock = new StockProducto
                    {
                        IdProducto = productoId,
                        CantidadDisponible = Math.Max(0, cantidad),
                        FechaActualizacion = DateTime.UtcNow
                    };
                    await _repository.Add(stock);
                }
                else
                {
                    // Actualizar stock existente
                    stock.CantidadDisponible = Math.Max(0, stock.CantidadDisponible + cantidad);
                    stock.FechaActualizacion = DateTime.UtcNow;
                    await _repository.Update(stock);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> ObtenerStockAsync(int productoId)
        {
            var stocks = await _repository.GetAllAsync(
                filter: s => s.IdProducto == productoId
            );

            var stock = stocks.FirstOrDefault();
            return stock?.CantidadDisponible ?? 0;
        }

        public async Task<bool> ValidarDisponibilidadAsync(List<(int productoId, int cantidad)> productos)
        {
            foreach (var (productoId, cantidad) in productos)
            {
                var stockDisponible = await ObtenerStockAsync(productoId);
                if (stockDisponible < cantidad)
                    return false;
            }
            return true;
        }
    }
}