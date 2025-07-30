using Microsoft.Extensions.Logging;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    /// <summary>
    /// Implementa RF5 
    /// RF5: Consultar disponibilidad de stock de productos
    /// Corresponde a ComponenteStock de la tabla de componentes
    /// </summary>
    public class ComponenteStock : IComponenteStock
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly ILogger<ComponenteStock> _logger;

        public ComponenteStock(
            IGenericRepository<Producto> productoRepository,
            ILogger<ComponenteStock> logger)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        /// <summary>
        /// RF5: Consultar disponibilidad de stock de productos
        /// Funcionalidad: verificarDisponibilidad(productoId, cantidad)
        /// </summary>
        public async Task<DisponibilidadStockDto> ConsultarDisponibilidadAsync(int productoId, int cantidad)
        {
            try
            {
                _logger.LogInformation("Consultando disponibilidad del producto {ProductoId} para cantidad {Cantidad}",
                    productoId, cantidad);

                var producto = await _productoRepository.Get(productoId);
                if (producto == null)
                {
                    return new DisponibilidadStockDto
                    {
                        ProductoId = productoId,
                        CantidadSolicitada = cantidad,
                        Disponible = false,
                        Mensaje = "Producto no encontrado",
                        Cantidad = cantidad,
                        StockDisponible = 0,
                        NombreProducto = "Producto no encontrado"
                    };
                }

                var disponible = producto.EstaDisponible(cantidad);
                var mensaje = disponible
                    ? $"Stock disponible: {producto.StockDisponible} unidades. Cantidad solicitada: {cantidad} ✅"
                    : $"Stock insuficiente. Disponible: {producto.StockDisponible}, Solicitado: {cantidad} ❌";

                _logger.LogInformation("Consulta de disponibilidad - Producto: {ProductoId}, Disponible: {Disponible}",
                    productoId, disponible);

                return new DisponibilidadStockDto
                {
                    ProductoId = productoId,
                    NombreProducto = producto.Nombre,
                    StockDisponible = producto.StockDisponible,
                    CantidadSolicitada = cantidad,
                    Disponible = disponible,
                    Mensaje = mensaje,
                    Cantidad = cantidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar disponibilidad del producto {ProductoId}", productoId);
                return new DisponibilidadStockDto
                {
                    ProductoId = productoId,
                    CantidadSolicitada = cantidad,
                    Disponible = false,
                    Mensaje = "Error al consultar disponibilidad",
                    Cantidad = cantidad
                };
            }
        }

       

        /// <summary>
        /// Funcionalidad: obtenerStock(productoId)
        /// Obtiene el stock disponible de un producto
        /// </summary>
        public async Task<int> ObtenerStockAsync(int productoId)
        {
            try
            {
                var producto = await _productoRepository.Get(productoId);
                return producto?.StockDisponible ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener stock del producto {ProductoId}", productoId);
                return 0;
            }
        }

        /// <summary>
        /// Reabastecer stock de un producto
        /// Funcionalidad complementaria para gestión de inventario
        /// </summary>
        public async Task<bool> ReabastecerStockAsync(int productoId, int cantidad)
        {
            try
            {
                _logger.LogInformation("Reabasteciendo producto {ProductoId} con {Cantidad} unidades",
                    productoId, cantidad);

                var producto = await _productoRepository.Get(productoId);
                if (producto == null)
                {
                    _logger.LogWarning("Producto {ProductoId} no encontrado para reabastecer", productoId);
                    return false;
                }

                if (cantidad <= 0)
                {
                    _logger.LogWarning("Cantidad de reabastecimiento inválida: {Cantidad}", cantidad);
                    return false;
                }

                // Reabastecer usando método de la entidad
                producto.reabastecer(cantidad);
                await _productoRepository.Update(producto);

                _logger.LogInformation("Producto {ProductoId} reabastecido. Nuevo stock: {NuevoStock}",
                    productoId, producto.StockDisponible);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reabastecer producto {ProductoId}", productoId);
                return false;
            }
        }

        /// <summary>
        /// Obtiene productos con stock bajo el mínimo especificado
        /// Útil para alertas de reabastecimiento
        /// </summary>
        public async Task<IEnumerable<Producto>> ObtenerProductosBajoStockAsync(int stockMinimo = 5)
        {
            try
            {
                return await _productoRepository.GetAllAsync(
                    filter: p => p.Activo && p.StockDisponible <= stockMinimo,
                    orderBy: q => q.OrderBy(p => p.StockDisponible)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos con bajo stock");
                return new List<Producto>();
            }
        }

        /// <summary>
        /// Obtiene productos con stock disponible
        /// </summary>
        public async Task<IEnumerable<Producto>> ObtenerProductosDisponiblesAsync()
        {
            try
            {
                return await _productoRepository.GetAllAsync(
                    filter: p => p.Activo && p.StockDisponible > 0,
                    orderBy: q => q.OrderBy(p => p.Nombre)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos disponibles");
                return new List<Producto>();
            }
        }
    }
}