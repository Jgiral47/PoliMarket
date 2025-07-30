

// ProductosController.cs
using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Contracts;

namespace PoliMarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IComponenteProducto _componenteProducto;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IComponenteProducto componenteProducto, ILogger<ProductosController> logger)
        {
            _componenteProducto = componenteProducto;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListarProductos()
        {
            try
            {
                var productos = await _componenteProducto.ListarProductosAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerProducto(int productoId)
        {
            try
            {
                var producto = await _componenteProducto.ObtenerProductoPorIdAsync(productoId);
                if (producto == null)
                    return NotFound(new { mensaje = "Producto no encontrado" });

                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {ProductoId}", productoId);
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarProductos([FromQuery] string termino = "")
        {
            try
            {
                var productos = await _componenteProducto.BuscarProductosAsync(termino);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}