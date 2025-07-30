using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;
using PoliMarket.Core.Entities;

namespace PoliMarket.API.Controllers
{
    [Route("api/[controller]")]
    public class ProveedorController : BaseController<Proveedor, IProveedorService>
    {
        public ProveedorController(IProveedorService service) : base(service) { }

        protected override object GetEntityId(Proveedor entity)
        {
            return entity.IdProveedor;
        }

        /// <summary>
        /// RF7: Consultar qué productos suministra cada proveedor
        /// </summary>
        [HttpGet("{id}/productos")]
        public async Task<ActionResult<List<Producto>>> ListarProductosSuministrados(int id)
        {
            try
            {
                var productos = await _service.ListarProductosSuministradosAsync(id);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtener información de suministro de un producto específico
        /// </summary>
        [HttpGet("{proveedorId}/producto/{productoId}/info")]
        public async Task<ActionResult<object>> ObtenerInfoSuministro(int proveedorId, int productoId)
        {
            try
            {
                var info = await _service.ObtenerInfoSuministroAsync(proveedorId, productoId);
                return Ok(new
                {
                    proveedorId = proveedorId,
                    productoId = productoId,
                    informacion = info
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}