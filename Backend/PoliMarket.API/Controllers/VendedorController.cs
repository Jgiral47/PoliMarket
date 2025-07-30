// VentasController.cs - RF2 y RF3
using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Contracts;
using PoliMarket.Business.DTOs;

namespace PoliMarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IComponenteVentas _componenteVentas;
        private readonly ILogger<VentasController> _logger;

        public VentasController(IComponenteVentas componenteVentas, ILogger<VentasController> logger)
        {
            _componenteVentas = componenteVentas;
            _logger = logger;
        }

        /// <summary>
        /// RF2: Registrar una nueva venta
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] RegistrarVentaRequest request)
        {
            try
            {
                var venta = await _componenteVentas.RegistrarVentaAsync(
                    request.IdVendedor,
                    request.IdCliente,
                    request.Productos);

                return Ok(new { success = true, ventaId = venta.IdVenta, mensaje = "Venta registrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar venta");
                return BadRequest(new { success = false, mensaje = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Calcular total de una venta
        /// </summary>
        [HttpGet("{ventaId}/total")]
        public async Task<IActionResult> CalcularTotal(int ventaId)
        {
            try
            {
                var total = await _componenteVentas.CalcularTotalAsync(ventaId);
                return Ok(new { ventaId, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular total de venta {VentaId}", ventaId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Generar factura de una venta
        /// </summary>
        [HttpGet("{ventaId}/factura")]
        public async Task<IActionResult> GenerarFactura(int ventaId)
        {
            try
            {
                var factura = await _componenteVentas.GenerarFacturaAsync(ventaId);
                return Ok(factura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar factura de venta {VentaId}", ventaId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Agregar producto a venta existente
        /// </summary>
        [HttpPost("{ventaId}/productos")]
        public async Task<IActionResult> AgregarProducto(int ventaId, [FromBody] AgregarProductoRequest request)
        {
            try
            {
                var resultado = await _componenteVentas.AgregarProductoAsync(ventaId, request.IdProducto, request.Cantidad);
                return Ok(new { success = resultado, mensaje = resultado ? "Producto agregado" : "Error al agregar producto" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto a venta {VentaId}", ventaId);
                return BadRequest(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpGet("vendedor/{vendedorId}")]
        public async Task<IActionResult> ObtenerVentasPorVendedor(int vendedorId)
        {
            try
            {
                var ventas = await _componenteVentas.ObtenerVentasPorVendedorAsync(vendedorId);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas del vendedor {VendedorId}", vendedorId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{ventaId}")]
        public async Task<IActionResult> ObtenerVentaCompleta(int ventaId)
        {
            try
            {
                var venta = await _componenteVentas.ObtenerVentaCompletaAsync(ventaId);
                if (venta == null)
                    return NotFound(new { mensaje = "Venta no encontrada" });

                return Ok(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener venta {VentaId}", ventaId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{ventaId}/completar")]
        public async Task<IActionResult> CompletarVenta(int ventaId)
        {
            try
            {
                var resultado = await _componenteVentas.CompletarVentaAsync(ventaId);
                return Ok(new { success = resultado, mensaje = resultado ? "Venta completada" : "Error al completar venta" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar venta {VentaId}", ventaId);
                return BadRequest(new { success = false, mensaje = ex.Message });
            }
        }
    }

    // DTOs para requests
    public class RegistrarVentaRequest
    {
        public int IdVendedor { get; set; }
        public int IdCliente { get; set; }
        public List<ProductoVentaDto> Productos { get; set; } = new();
    }

    public class AgregarProductoRequest
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }
}

