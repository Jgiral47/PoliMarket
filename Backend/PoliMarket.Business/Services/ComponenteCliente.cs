using Microsoft.Extensions.Logging;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    /// <summary>
    /// ComponenteCliente - Implementa RF4
    /// RF4: Consultar historial de órdenes del cliente
    /// Corresponde a ComponenteCliente de la tabla de componentes
    /// </summary>
    public class ComponenteCliente : IComponenteCliente
    {
        private readonly IGenericRepository<Cliente> _clienteRepository;
        private readonly IGenericRepository<Venta> _ventaRepository;
        private readonly IGenericRepository<VentaProducto> _ventaProductoRepository;
        private readonly ILogger<ComponenteCliente> _logger;

        public ComponenteCliente(
            IGenericRepository<Cliente> clienteRepository,
            IGenericRepository<Venta> ventaRepository,
            IGenericRepository<VentaProducto> ventaProductoRepository,
            ILogger<ComponenteCliente> logger)
        {
            _clienteRepository = clienteRepository;
            _ventaRepository = ventaRepository;
            _ventaProductoRepository = ventaProductoRepository;
            _logger = logger;
        }

        /// <summary>
        /// RF4: Consultar historial de órdenes del cliente
        /// Funcionalidad: consultarHistorialOrdenes(), listarProductosDisponibles()
        /// </summary>
        public async Task<IEnumerable<Venta>> ConsultarHistorialOrdenesAsync(int clienteId)
        {
            try
            {
                _logger.LogInformation("Consultando historial de órdenes para cliente {ClienteId}", clienteId);

                // Validar que el cliente existe
                var cliente = await _clienteRepository.Get(clienteId);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente {ClienteId} no encontrado", clienteId);
                    return Enumerable.Empty<Venta>();
                }

                // Obtener todas las ventas del cliente
                var ventas = await _ventaRepository.GetAllAsync(
                    filter: v => v.IdCliente == clienteId,
                    orderBy: q => q.OrderByDescending(v => v.FechaVenta)
                );

                // Cargar los productos para cada venta
                foreach (var venta in ventas)
                {
                    var ventaProductos = await _ventaProductoRepository.GetAllAsync(
                        filter: vp => vp.IdVenta == venta.IdVenta
                    );
                    venta.VentaProductos = ventaProductos.ToList();
                }

                _logger.LogInformation("Historial obtenido: {Count} órdenes para cliente {ClienteId}",
                    ventas.Count(), clienteId);

                return ventas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar historial de órdenes para cliente {ClienteId}", clienteId);
                return Enumerable.Empty<Venta>();
            }
        }

        /// <summary>
        /// Obtener cliente por ID
        /// </summary>
        public async Task<Cliente?> ObtenerClientePorIdAsync(int clienteId)
        {
            try
            {
                return await _clienteRepository.Get(clienteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {ClienteId}", clienteId);
                return null;
            }
        }

        /// <summary>
        /// Obtener cliente por código
        /// </summary>
        public async Task<Cliente?> ObtenerClientePorCodigoAsync(int codigoCliente)
        {
            try
            {
                var clientes = await _clienteRepository.GetAllAsync(
                    filter: c => c.CodigoCliente == codigoCliente
                );
                return clientes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente por código {CodigoCliente}", codigoCliente);
                return null;
            }
        }

        /// <summary>
        /// Listar clientes activos
        /// </summary>
        public async Task<IEnumerable<Cliente>> ListarClientesActivosAsync()
        {
            try
            {
                return await _clienteRepository.GetAllAsync(
                    filter: c => c.Activo,
                    orderBy: q => q.OrderBy(c => c.Nombre).ThenBy(c => c.Apellido)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes activos");
                return Enumerable.Empty<Cliente>();
            }
        }

        /// <summary>
        /// Verificar si un cliente tiene compras activas (ventas no canceladas)
        /// </summary>
        public async Task<bool> TieneComprasActivasAsync(int clienteId)
        {
            try
            {
                var ventasActivas = await _ventaRepository.GetAllAsync(
                    filter: v => v.IdCliente == clienteId && v.Estado != "Cancelada"
                );

                return ventasActivas.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar compras activas para cliente {ClienteId}", clienteId);
                return false;
            }
        }
    }
}