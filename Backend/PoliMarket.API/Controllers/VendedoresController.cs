// VendedoresController.cs - RF1
using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Contracts;

namespace PoliMarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendedoresController : ControllerBase
    {
        private readonly IComponenteVendedor _componenteVendedor;
        private readonly ILogger<VendedoresController> _logger;

        public VendedoresController(IComponenteVendedor componenteVendedor, ILogger<VendedoresController> logger)
        {
            _componenteVendedor = componenteVendedor;
            _logger = logger;
        }

        /// <summary>
        /// RF1: Autorizar vendedor
        /// </summary>
        [HttpPost("{vendedorId}/autorizar")]
        public async Task<IActionResult> AutorizarVendedor(int vendedorId, [FromBody] AutorizarVendedorRequest request)
        {
            try
            {
                var resultado = await _componenteVendedor.AutorizarVendedorAsync(vendedorId, request.AutorizacionId);
                return Ok(new
                {
                    success = resultado,
                    mensaje = resultado ? "Vendedor autorizado exitosamente" : "Error al autorizar vendedor"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autorizar vendedor {VendedorId}", vendedorId);
                return BadRequest(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpGet("{vendedorId}/estado-autorizacion")]
        public async Task<IActionResult> ObtenerEstadoAutorizacion(int vendedorId)
        {
            try
            {
                var estado = await _componenteVendedor.ObtenerEstadoAutorizacionAsync(vendedorId);
                return Ok(estado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado de autorización del vendedor {VendedorId}", vendedorId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarVendedores()
        {
            try
            {
                var vendedores = await _componenteVendedor.ListarVendedoresAsync();
                return Ok(vendedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar vendedores");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("autorizados")]
        public async Task<IActionResult> ListarVendedoresAutorizados()
        {
            try
            {
                var vendedores = await _componenteVendedor.ListarVendedoresAutorizadosAsync();
                return Ok(vendedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar vendedores autorizados");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }

    public class AutorizarVendedorRequest
    {
        public int AutorizacionId { get; set; }
    }
}