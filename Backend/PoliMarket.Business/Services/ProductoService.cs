using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class ProductoService : BaseService<Producto>, IProductoService
    {
        private readonly IGenericRepository<StockProducto> _stockRepository;

        public ProductoService(
            IGenericRepository<Producto> repository,
            IGenericRepository<StockProducto> stockRepository)
            : base(repository)
        {
            _stockRepository = stockRepository;
        }

        // RF5: Consultar disponibilidad de stock de productos
        public async Task<bool> EstaDisponibleAsync(int productoId, int cantidad)
        {
            var stock = await _stockRepository.GetAllAsync(
                filter: s => s.IdProducto == productoId
            );

            var stockProducto = stock.FirstOrDefault();
            return stockProducto?.CantidadDisponible >= cantidad;
        }

        public async Task<double> CalcularSubtotalAsync(int productoId, int cantidad)
        {
            var producto = await _repository.Get(productoId);
            if (producto == null) return 0;

            return producto.Precio * cantidad;
        }
    }
}