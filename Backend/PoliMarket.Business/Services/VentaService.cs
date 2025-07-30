using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public class VentaService : BaseService<Venta>, IVentaService
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IGenericRepository<ProductoVenta> _productoVentaRepository;
        private readonly IStockService _stockService;

        public VentaService(
            IGenericRepository<Venta> repository,
            IGenericRepository<Producto> productoRepository,
            IGenericRepository<ProductoVenta> productoVentaRepository,
            IStockService stockService)
            : base(repository)
        {
            _productoRepository = productoRepository;
            _productoVentaRepository = productoVentaRepository;
            _stockService = stockService;
        }

        // RF2: Registrar ventas de productos a clientes
        public async Task<Venta> RegistrarVentaAsync(int vendedorId, int clienteId, List<(int productoId, int cantidad)> productos)
        {
            // Validar disponibilidad de productos
            var disponible = await _stockService.ValidarDisponibilidadAsync(productos);
            if (!disponible)
                throw new InvalidOperationException("Stock insuficiente para algunos productos");

            // Crear nueva venta
            var venta = new Venta
            {
                IdVendedor = vendedorId,
                IdCliente = clienteId,
                FechaVenta = DateTime.UtcNow,
                Estado = "PENDIENTE"
            };

            var ventaCreada = await _repository.Add(venta);

            // Agregar productos a la venta
            foreach (var (productoId, cantidad) in productos)
            {
                var producto = await _productoRepository.Get(productoId);
                if (producto != null)
                {
                    var productoVenta = new ProductoVenta
                    {
                        IdVenta = ventaCreada.IdVenta,
                        IdProducto = productoId,
                        Cantidad = cantidad,
                        PrecioUnitario = producto.Precio
                    };

                    await _productoVentaRepository.Add(productoVenta);

                    // Actualizar stock
                    await _stockService.ActualizarStockAsync(productoId, -cantidad);
                }
            }

            return ventaCreada;
        }

        // RF3: Calcular totales, generar factura y agregar productos a una venta
        public async Task<double> CalcularTotalAsync(int ventaId)
        {
            var productosVenta = await _productoVentaRepository.GetAllAsync(
                filter: pv => pv.IdVenta == ventaId
            );

            return productosVenta.Sum(pv => pv.CalcularSubtotal());
        }

        public async Task<string> GenerarFacturaAsync(int ventaId)
        {
            var venta = await _repository.Get(ventaId);
            if (venta == null) return string.Empty;

            var total = await CalcularTotalAsync(ventaId);
            return $"Factura #{ventaId} - Cliente: {venta.IdCliente} - Total: {total:C} - Fecha: {venta.FechaVenta:dd/MM/yyyy}";
        }

        public async Task<bool> AgregarProductoAsync(int ventaId, int productoId, int cantidad)
        {
            try
            {
                var producto = await _productoRepository.Get(productoId);
                if (producto == null) return false;

                // Verificar disponibilidad
                var disponible = await _stockService.ObtenerStockAsync(productoId);
                if (disponible < cantidad) return false;

                var productoVenta = new ProductoVenta
                {
                    IdVenta = ventaId,
                    IdProducto = productoId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                };

                await _productoVentaRepository.Add(productoVenta);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}