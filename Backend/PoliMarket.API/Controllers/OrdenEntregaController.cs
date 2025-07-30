using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class OrdenEntregaController : BaseController<OrdenEntrega, IOrdenEntregaService>
    {
        public OrdenEntregaController(IOrdenEntregaService service) : base(service) { }

        protected override object GetEntityId(OrdenEntrega entity)
        {
            return entity.IdOrden;
        }

        /// <summary>
        /// RF8: Registrar entrega y actualizar estado de la orden
        /// </summary>
        [HttpPut("{id}/registrar-entrega")]
        public async Task<ActionResult<bool>> RegistrarEntrega(int id, [FromQuery] string nuevoEstado)
        {
            try
            {
                var resultado = await _service.RegistrarEntregaAsync(id, nuevoEstado);
                if (resultado)
                    return Ok(new { message = "Entrega registrada exitosamente", ordenId = id, estado = nuevoEstado });

                return BadRequest(new { message = "No se pudo registrar la entrega" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Generar resumen de una orden
        /// </summary>
        [HttpGet("{id}/resumen")]
        public async Task<ActionResult<object>> GenerarResumen(int id)
        {
            try
            {
                var resumen = await _service.GenerarResumenAsync(id);
                return Ok(new
                {
                    ordenId = id,
                    resumen = resumen
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF9: Ver transiciones de estado de una orden (historial de cambios)
        /// </summary>
        [HttpGet("{id}/historial")]
        public async Task<ActionResult<List<HistoricoOrdenEntrega>>> VerTransiciones(int id)
        {
            try
            {
                var transiciones = await _service.VerTransicionesAsync(id);
                return Ok(transiciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}