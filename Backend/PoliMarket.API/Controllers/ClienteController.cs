using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class ClienteController : BaseController<Cliente, IClienteService>
    {
        public ClienteController(IClienteService service) : base(service) { }

        protected override object GetEntityId(Cliente entity)
        {
            return entity.Id;
        }

        /// <summary>
        /// RF4: Consultar historial de órdenes del cliente
        /// </summary>
        [HttpGet("{id}/historial-ordenes")]
        public async Task<ActionResult<List<OrdenEntrega>>> ConsultarHistorialOrdenes(int id)
        {
            try
            {
                var ordenes = await _service.ConsultarHistorialOrdenesAsync(id);
                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Listar productos disponibles
        /// </summary>
        [HttpGet("productos-disponibles")]
        public async Task<ActionResult<List<Producto>>> ListarProductosDisponibles()
        {
            try
            {
                var productos = await _service.ListarProductosDisponiblesAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}