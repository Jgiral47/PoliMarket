using Microsoft.Extensions.Logging;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    /// <summary>
    /// ComponenteProducto - Operaciones sobre productos
    /// Funcionalidades complementarias para gestión de productos
    /// </summary>
    public class ComponenteProducto : IComponenteProducto
    {
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly ILogger<ComponenteProducto> _logger;

        public ComponenteProducto(
            IGenericRepository<Producto> productoRepository,
            ILogger<ComponenteProducto> logger)
        {
            _productoRepository = productoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Listar todos los productos activos
        /// </summary>
        public async Task<IEnumerable<Producto>> ListarProductosAsync()
        {
            try
            {
                return await _productoRepository.GetAllAsync(
                    filter: p => p.Activo,
                    orderBy: q => q.OrderBy(p => p.Nombre)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                return Enumerable.Empty<Producto>();
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        public async Task<Producto?> ObtenerProductoPorIdAsync(int productoId)
        {
            try
            {
                return await _productoRepository.Get(productoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {ProductoId}", productoId);
                return null;
            }
        }

        /// <summary>
        /// Verificar si un producto está disponible en la cantidad solicitada
        /// </summary>
        public async Task<bool> EstaDisponibleAsync(int productoId, int cantidad)
        {
            try
            {
                var producto = await _productoRepository.Get(productoId);
                return producto?.EstaDisponible(cantidad) ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad del producto {ProductoId}", productoId);
                return false;
            }
        }

        /// <summary>
        /// Calcular subtotal de un producto por cantidad
        /// </summary>
        public async Task<decimal> CalcularSubtotalAsync(int productoId, int cantidad)
        {
            try
            {
                var producto = await _productoRepository.Get(productoId);
                if (producto == null) return 0;

                return producto.CalcularSubtotal(cantidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular subtotal del producto {ProductoId}", productoId);
                return 0;
            }
        }

        /// <summary>
        /// Buscar productos por término de búsqueda
        /// </summary>
        public async Task<IEnumerable<Producto>> BuscarProductosAsync(string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return await ListarProductosAsync();
                }

                return await _productoRepository.GetAllAsync(
                    filter: p => p.Activo &&
                               (p.Nombre.Contains(termino) ||
                                p.Descripcion.Contains(termino)),
                    orderBy: q => q.OrderBy(p => p.Nombre)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con término {Termino}", termino);
                return Enumerable.Empty<Producto>();
            }
        }
    }
}