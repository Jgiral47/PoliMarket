
// ClientesController.cs - RF4
using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Contracts;

namespace PoliMarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IComponenteCliente _componenteCliente;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IComponenteCliente componenteCliente, ILogger<ClientesController> logger)
        {
            _componenteCliente = componenteCliente;
            _logger = logger;
        }

        /// <summary>
        /// RF4: Consultar historial de órdenes del cliente
        /// </summary>
        [HttpGet("{clienteId}/historial-ordenes")]
        public async Task<IActionResult> ConsultarHistorialOrdenes(int clienteId)
        {
            try
            {
                var historial = await _componenteCliente.ConsultarHistorialOrdenesAsync(clienteId);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar historial de cliente {ClienteId}", clienteId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{clienteId}")]
        public async Task<IActionResult> ObtenerCliente(int clienteId)
        {
            try
            {
                var cliente = await _componenteCliente.ObtenerClientePorIdAsync(clienteId);
                if (cliente == null)
                    return NotFound(new { mensaje = "Cliente no encontrado" });

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente {ClienteId}", clienteId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarClientesActivos()
        {
            try
            {
                var clientes = await _componenteCliente.ListarClientesActivosAsync();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes activos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
