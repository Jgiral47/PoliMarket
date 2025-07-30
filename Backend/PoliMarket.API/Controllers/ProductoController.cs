using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class ProductoController : BaseController<Producto, IProductoService>
    {
        public ProductoController(IProductoService service) : base(service) { }

        protected override object GetEntityId(Producto entity)
        {
            return entity.IdProducto;
        }

        /// <summary>
        /// RF5: Consultar disponibilidad de stock de productos
        /// </summary>
        [HttpGet("{id}/disponibilidad")]
        public async Task<ActionResult<object>> ConsultarDisponibilidad(int id, [FromQuery] int cantidad = 1)
        {
            try
            {
                var disponible = await _service.EstaDisponibleAsync(id, cantidad);
                return Ok(new
                {
                    productoId = id,
                    cantidad = cantidad,
                    disponible = disponible,
                    mensaje = disponible ? "Producto disponible" : "Stock insuficiente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Calcular subtotal para una cantidad de producto
        /// </summary>
        [HttpGet("{id}/subtotal")]
        public async Task<ActionResult<object>> CalcularSubtotal(int id, [FromQuery] int cantidad)
        {
            try
            {
                var subtotal = await _service.CalcularSubtotalAsync(id, cantidad);
                return Ok(new
                {
                    productoId = id,
                    cantidad = cantidad,
                    subtotal = subtotal
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}