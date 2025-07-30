using Microsoft.AspNetCore.Mvc;
using PoliMarket.API.DTOs.Requests;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class VendedorController : BaseController<Vendedor, IVendedorService>
    {
        public VendedorController(IVendedorService service) : base(service) { }

        protected override object GetEntityId(Vendedor entity)
        {
            return entity.Id;
        }

        /// <summary>
        /// RF1: Autorizar vendedor para operar en el sistema
        /// </summary>
        [HttpPost("{id}/autorizar")]
        public async Task<ActionResult<bool>> AutorizarVendedor(int id, [FromBody] AutorizarVendedorRequest request)
        {
            try
            {
                var resultado = await _service.AutorizarVendedorAsync(id, request.IdAutorizacion);
                if (resultado)
                    return Ok(new { message = "Vendedor autorizado exitosamente", autorizado = true });

                return BadRequest(new { message = "No se pudo autorizar el vendedor" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Verificar si un vendedor está autorizado
        /// </summary>
        [HttpGet("{id}/estado-autorizacion")]
        public async Task<ActionResult<object>> ObtenerEstadoAutorizacion(int id)
        {
            try
            {
                var autorizado = await _service.EstaAutorizadoAsync(id);
                return Ok(new
                {
                    vendedorId = id,
                    autorizado = autorizado,
                    estado = autorizado ? "AUTORIZADO" : "PENDIENTE"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Listar clientes disponibles para un vendedor
        /// </summary>
        [HttpGet("{id}/clientes")]
        public async Task<ActionResult<List<Cliente>>> ListarClientes(int id)
        {
            try
            {
                var clientes = await _service.ListarClientesAsync(id);
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}