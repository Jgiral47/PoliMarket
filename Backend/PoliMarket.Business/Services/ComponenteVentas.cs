using Microsoft.Extensions.Logging;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;
using PoliMarket.Core.Entities;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    /// <summary>
    /// ComponenteVentas - Implementa RF2 y RF3
    /// RF2: Registrar ventas de productos a clientes
    /// RF3: Calcular totales, generar factura y agregar productos a una venta
    /// Corresponde a ComponenteVentas de la tabla de componentes
    /// </summary>
    public class ComponenteVentas : IComponenteVentas
    {
        private readonly IGenericRepository<Venta> _ventaRepository;
        private readonly IGenericRepository<VentaProducto> _ventaProductoRepository;
        private readonly IGenericRepository<Producto> _productoRepository;
        private readonly IGenericRepository<Vendedor> _vendedorRepository;
        private readonly IGenericRepository<Cliente> _clienteRepository;
        private readonly IComponenteStock _componenteStock;
        private readonly ILogger<ComponenteVentas> _logger;

        public ComponenteVentas(
            IGenericRepository<Venta> ventaRepository,
            IGenericRepository<VentaProducto> ventaProductoRepository,
            IGenericRepository<Producto> productoRepository,
            IGenericRepository<Vendedor> vendedorRepository,
            IGenericRepository<Cliente> clienteRepository,
            IComponenteStock componenteStock,
            ILogger<ComponenteVentas> logger)
        {
            _ventaRepository = ventaRepository;
            _ventaProductoRepository = ventaProductoRepository;
            _productoRepository = productoRepository;
            _vendedorRepository = vendedorRepository;
            _clienteRepository = clienteRepository;
            _componenteStock = componenteStock;
            _logger = logger;
        }

        /// <summary>
        /// RF2: Registrar ventas de productos a clientes
        /// Funcionalidad: registrarVenta(cliente, productos, cantidades)
        /// </summary>
        public async Task<Venta> RegistrarVentaAsync(int idVendedor, int idCliente, List<ProductoVentaDto> productos)
        {
            try
            {
                _logger.LogInformation("Iniciando registro de venta - Vendedor: {VendedorId}, Cliente: {ClienteId}",
                    idVendedor, idCliente);

                // Validar vendedor autorizado
                var vendedor = await _vendedorRepository.Get(idVendedor);
                if (vendedor == null || !vendedor.PuedeVender())
                {
                    throw new InvalidOperationException("Vendedor no autorizado para realizar ventas");
                }

                // Validar cliente
                var cliente = await _clienteRepository.Get(idCliente);
                if (cliente == null || !cliente.Activo)
                {
                    throw new InvalidOperationException("Cliente no válido o inactivo");
                }

                // Verificar disponibilidad de productos
                foreach (var item in productos)
                {
                    var disponibilidad = await _componenteStock.ConsultarDisponibilidadAsync(item.IdProducto, item.Cantidad);
                    if (!disponibilidad.Disponible)
                    {
                        throw new InvalidOperationException($"Producto {item.IdProducto} no tiene stock suficiente");
                    }
                }

                // Crear venta
                var todasLasVentas = await _ventaRepository.GetAllAsync();
                var proximoIdVenta = (todasLasVentas.Any() ? todasLasVentas.Max(v => v.IdVenta) : 7000) + 1;

                var venta = new Venta
                {
                    IdVenta = proximoIdVenta,
                    FechaVenta = DateTime.Now,
                    Estado = "Pendiente",
                    IdVendedor = idVendedor,
                    IdCliente = idCliente,
                    Vendedor = vendedor,
                    Cliente = cliente,
                    VentaProductos = new List<VentaProducto>()
                };

                // Guardar venta inicial
                await _ventaRepository.Add(venta);

                // Agregar productos a la venta
                foreach (var item in productos)
                {
                    await AgregarProductoAsync(venta.IdVenta, item.IdProducto, item.Cantidad);
                }

                // Completar venta (calcular total)
                await CompletarVentaAsync(venta.IdVenta);
                var ventaCompleta = await ObtenerVentaCompletaAsync(venta.IdVenta);

                _logger.LogInformation("Venta registrada exitosamente - ID: {VentaId}, Total: {Total}",
                    ventaCompleta?.IdVenta, ventaCompleta?.Total);

                return ventaCompleta ?? venta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar venta - Vendedor: {VendedorId}, Cliente: {ClienteId}",
                    idVendedor, idCliente);
                throw;
            }
        }

        /// <summary>
        /// RF3: Calcular totales de una venta
        /// Funcionalidad: calcularTotal()
        /// </summary>
        public async Task<decimal> CalcularTotalAsync(int ventaId)
        {
            try
            {
                _logger.LogInformation("Calculando total de venta - VentaId: {VentaId}", ventaId);

                var ventaProductos = await _ventaProductoRepository.GetAllAsync(
                    filter: vp => vp.IdVenta == ventaId
                );

                if (!ventaProductos.Any())
                {
                    _logger.LogWarning("No se encontraron productos para la venta {VentaId}", ventaId);
                    return 0;
                }

                decimal total = ventaProductos.Sum(vp => vp.Subtotal);

                _logger.LogInformation("Total calculado para venta {VentaId}: {Total}", ventaId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular total de venta - VentaId: {VentaId}", ventaId);
                throw;
            }
        }

        /// <summary>
        /// RF3: Generar factura de una venta
        /// Funcionalidad: generarFactura()
        /// </summary>
        public async Task<FacturaDto> GenerarFacturaAsync(int ventaId)
        {
            try
            {
                _logger.LogInformation("Generando factura para venta - VentaId: {VentaId}", ventaId);

                var venta = await _ventaRepository.Get(ventaId);
                if (venta == null)
                {
                    throw new InvalidOperationException("Venta no encontrada");
                }

                var ventaProductos = await _ventaProductoRepository.GetAllAsync(
                    filter: vp => vp.IdVenta == ventaId
                );

                var vendedor = await _vendedorRepository.Get(venta.IdVendedor);
                var cliente = await _clienteRepository.Get(venta.IdCliente);

                var detallesFactura = new List<DetalleFacturaDto>();
                foreach (var vp in ventaProductos)
                {
                    var producto = await _productoRepository.Get(vp.IdProducto);
                    detallesFactura.Add(new DetalleFacturaDto
                    {
                        Producto = producto?.Nombre ?? "Producto no encontrado",
                        Cantidad = vp.Cantidad,
                        PrecioUnitario = vp.PrecioUnitario,
                        Subtotal = vp.Subtotal
                    });
                }

                // Generar número de factura único
                var numeroFactura = $"FAC-{ventaId:D6}-{DateTime.Now:yyyyMMdd}";

                var factura = new FacturaDto
                {
                    NumeroFactura = numeroFactura,
                    FechaFactura = venta.FechaVenta,
                    VentaId = ventaId,
                    Cliente = $"{cliente?.Nombre} {cliente?.Apellido}",
                    Vendedor = $"{vendedor?.Nombre} {vendedor?.Apellido}",
                    Detalles = detallesFactura,
                    Total = detallesFactura.Sum(d => d.Subtotal)
                };

                _logger.LogInformation("Factura generada exitosamente - Número: {NumeroFactura}, VentaId: {VentaId}",
                    numeroFactura, ventaId);
                return factura;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar factura - VentaId: {VentaId}", ventaId);
                throw;
            }
        }

        /// <summary>
        /// RF3: Agregar productos a una venta
        /// Funcionalidad: agregarProducto()
        /// </summary>
        public async Task<bool> AgregarProductoAsync(int ventaId, int idProducto, int cantidad)
        {
            try
            {
                _logger.LogInformation("Agregando producto a venta - VentaId: {VentaId}, ProductoId: {ProductoId}, Cantidad: {Cantidad}",
                    ventaId, idProducto, cantidad);

                // Validar venta existe
                var venta = await _ventaRepository.Get(ventaId);
                if (venta == null)
                {
                    throw new InvalidOperationException("Venta no encontrada");
                }

                // Validar producto existe
                var producto = await _productoRepository.Get(idProducto);
                if (producto == null)
                {
                    throw new InvalidOperationException("Producto no encontrado");
                }

                // Verificar disponibilidad
                var disponibilidad = await _componenteStock.ConsultarDisponibilidadAsync(idProducto, cantidad);
                if (!disponibilidad.Disponible)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto {producto.Nombre}");
                }

                // Crear relación VentaProducto
                var ventaProducto = new VentaProducto
                {
                    IdVenta = ventaId,
                    IdProducto = idProducto,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = cantidad * producto.Precio,
                    Venta = venta,
                    Producto = producto
                };

                await _ventaProductoRepository.Add(ventaProducto);

               
                _logger.LogInformation("Producto agregado exitosamente a la venta");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto a venta - VentaId: {VentaId}, ProductoId: {ProductoId}",
                    ventaId, idProducto);
                throw;
            }
        }

        /// <summary>
        /// Obtener ventas de un vendedor específico
        /// </summary>
        public async Task<IEnumerable<Venta>> ObtenerVentasPorVendedorAsync(int vendedorId)
        {
            try
            {
                var ventas = await _ventaRepository.GetAllAsync(
                    filter: v => v.IdVendedor == vendedorId,
                    orderBy: q => q.OrderByDescending(v => v.FechaVenta)
                );

                _logger.LogInformation("Obtenidas {Count} ventas para vendedor {VendedorId}",
                    ventas.Count(), vendedorId);

                return ventas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas por vendedor - VendedorId: {VendedorId}", vendedorId);
                return Enumerable.Empty<Venta>();
            }
        }

        /// <summary>
        /// Obtener ventas de un cliente específico
        /// </summary>
        public async Task<IEnumerable<Venta>> ObtenerVentasPorClienteAsync(int clienteId)
        {
            try
            {
                var ventas = await _ventaRepository.GetAllAsync(
                    filter: v => v.IdCliente == clienteId,
                    orderBy: q => q.OrderByDescending(v => v.FechaVenta)
                );

                _logger.LogInformation("Obtenidas {Count} ventas para cliente {ClienteId}",
                    ventas.Count(), clienteId);

                return ventas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas por cliente - ClienteId: {ClienteId}", clienteId);
                return Enumerable.Empty<Venta>();
            }
        }

        /// <summary>
        /// Obtener venta completa con productos incluidos
        /// </summary>
        public async Task<Venta?> ObtenerVentaCompletaAsync(int ventaId)
        {
            try
            {
                var venta = await _ventaRepository.Get(ventaId);
                if (venta == null)
                {
                    _logger.LogWarning("Venta no encontrada - VentaId: {VentaId}", ventaId);
                    return null;
                }

                // Cargar productos de la venta
                var ventaProductos = await _ventaProductoRepository.GetAllAsync(
                    filter: vp => vp.IdVenta == ventaId
                );
                venta.VentaProductos = ventaProductos.ToList();

                // Cargar vendedor y cliente
                venta.Vendedor = await _vendedorRepository.Get(venta.IdVendedor);
                venta.Cliente = await _clienteRepository.Get(venta.IdCliente);

                _logger.LogInformation("Venta completa obtenida - VentaId: {VentaId}, Productos: {ProductCount}",
                    ventaId, venta.VentaProductos?.Count ?? 0);

                return venta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener venta completa - VentaId: {VentaId}", ventaId);
                return null;
            }
        }

        /// <summary>
        /// Completar venta - actualiza estado y calcula total final
        /// </summary>
        public async Task<bool> CompletarVentaAsync(int ventaId)
        {
            try
            {
                var venta = await _ventaRepository.Get(ventaId);
                if (venta == null)
                {
                    _logger.LogWarning("Venta no encontrada al intentar completar - VentaId: {VentaId}", ventaId);
                    return false;
                }

                venta.Total = await CalcularTotalAsync(ventaId);
                venta.Estado = "Completada";

                await _ventaRepository.Update(venta);

                _logger.LogInformation("Venta completada - ID: {VentaId}, Total: {Total}", ventaId, venta.Total);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar venta - VentaId: {VentaId}", ventaId);
                return false;
            }
        }

        /// <summary>
        /// Cancelar una venta - actualiza estado a cancelada y restaura stock
        /// </summary>
        public async Task<bool> CancelarVentaAsync(int ventaId)
        {
            try
            {
                var venta = await _ventaRepository.Get(ventaId);
                if (venta == null)
                {
                    _logger.LogWarning("Venta no encontrada al intentar cancelar - VentaId: {VentaId}", ventaId);
                    return false;
                }

                if (venta.Estado == "Completada")
                {
                    _logger.LogWarning("No se puede cancelar una venta ya completada - VentaId: {VentaId}", ventaId);
                    return false;
                }

                // Restaurar stock de productos
                var ventaProductos = await _ventaProductoRepository.GetAllAsync(
                    filter: vp => vp.IdVenta == ventaId
                );

                foreach (var vp in ventaProductos)
                {
                    // Reabastecer el stock con la cantidad que se había vendido
                    await _componenteStock.ReabastecerStockAsync(vp.IdProducto, vp.Cantidad);
                }

                venta.Estado = "Cancelada";
                await _ventaRepository.Update(venta);

                _logger.LogInformation("Venta cancelada exitosamente - VentaId: {VentaId}", ventaId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar venta - VentaId: {VentaId}", ventaId);
                return false;
            }
        }
    }
}