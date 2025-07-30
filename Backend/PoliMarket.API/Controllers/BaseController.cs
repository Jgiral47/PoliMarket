using Microsoft.AspNetCore.Mvc;
using PoliMarket.Business.Interfaces;

namespace PoliMarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TEntity, TService> : ControllerBase
        where TEntity : class
        where TService : IBaseService<TEntity>
    {
        protected readonly TService _service;

        protected BaseController(TService service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<List<TEntity>>> GetAll()
        {
            try
            {
                var entities = await _service.ObtenerTodosAsync();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntity>> Get(int id)
        {
            try
            {
                var entity = await _service.ObtenerAsync(id);
                if (entity == null)
                    return NotFound(new { message = "Entidad no encontrada" });

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public virtual async Task<ActionResult<TEntity>> Create([FromBody] TEntity entity)
        {
            try
            {
                var created = await _service.CrearAsync(entity);
                return CreatedAtAction(nameof(Get), new { id = GetEntityId(created) }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TEntity>> Update(int id, [FromBody] TEntity entity)
        {
            try
            {
                var updated = await _service.ActualizarAsync(entity);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TEntity>> Delete(int id)
        {
            try
            {
                var deleted = await _service.EliminarAsync(id);
                if (deleted == null)
                    return NotFound(new { message = "Entidad no encontrada" });

                return Ok(new { message = "Entidad eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        protected abstract object GetEntityId(TEntity entity);
    }
}