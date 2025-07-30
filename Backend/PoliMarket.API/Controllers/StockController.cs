using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class StockController : BaseController<StockProducto, IStockService>
    {
        public StockController(IStockService service) : base(service) { }

        protected override object GetEntityId(StockProducto entity)
        {
            return entity.Id;
        }

        /// <summary>
        /// RF6: Actualizar stock de un producto
        /// </summary>
        [HttpPut("producto/{productoId}/actualizar")]
        public async Task<ActionResult<bool>> ActualizarStock(int productoId, [FromQuery] int cantidad)
        {
            try
            {
                var resultado = await _service.ActualizarStockAsync(productoId, cantidad);
                if (resultado)
                    return Ok(new { message = "Stock actualizado correctamente", productoId = productoId });

                return BadRequest(new { message = "No se pudo actualizar el stock" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF6: Obtener stock actual de un producto
        /// </summary>
        [HttpGet("producto/{productoId}")]
        public async Task<ActionResult<object>> ObtenerStock(int productoId)
        {
            try
            {
                var stock = await _service.ObtenerStockAsync(productoId);
                return Ok(new
                {
                    productoId = productoId,
                    stockDisponible = stock,
                    estado = stock > 0 ? "En stock" : "Agotado"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}