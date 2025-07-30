using Microsoft.AspNetCore.Mvc;
using PoliMarket.API.DTOs.Requests;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class VentaController : BaseController<Venta, IVentaService>
    {
        public VentaController(IVentaService service) : base(service) { }

        protected override object GetEntityId(Venta entity)
        {
            return entity.IdVenta;
        }

        /// <summary>
        /// RF2: Registrar venta de productos a clientes
        /// </summary>
        [HttpPost("vendedor/{vendedorId}/registrar")]
        public async Task<ActionResult<Venta>> RegistrarVenta(int vendedorId, [FromBody] RegistrarVentaRequest request)
        {
            try
            {
                var productos = request.Productos.Select(p => (p.IdProducto, p.Cantidad)).ToList();
                var venta = await _service.RegistrarVentaAsync(vendedorId, request.IdCliente, productos);

                return CreatedAtAction(nameof(Get), new { id = venta.IdVenta }, venta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Calcular total de una venta
        /// </summary>
        [HttpGet("{id}/total")]
        public async Task<ActionResult<object>> CalcularTotal(int id)
        {
            try
            {
                var total = await _service.CalcularTotalAsync(id);
                return Ok(new { ventaId = id, total = total });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Generar factura de una venta
        /// </summary>
        [HttpGet("{id}/factura")]
        public async Task<ActionResult<object>> GenerarFactura(int id)
        {
            try
            {
                var factura = await _service.GenerarFacturaAsync(id);
                return Ok(new { ventaId = id, factura = factura });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// RF3: Agregar producto a una venta existente
        /// </summary>
        [HttpPost("{id}/productos")]
        public async Task<ActionResult<bool>> AgregarProducto(int id, [FromBody] ProductoVentaRequest request)
        {
            try
            {
                var resultado = await _service.AgregarProductoAsync(id, request.IdProducto, request.Cantidad);
                if (resultado)
                    return Ok(new { message = "Producto agregado exitosamente" });

                return BadRequest(new { message = "No se pudo agregar el producto" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}