using PoliMarket.Business.Interfaces;
using PoliMarket.DataAccess.Contracts;

namespace PoliMarket.Business.Services
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly IGenericRepository<TEntity> _repository;

        protected BaseService(IGenericRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<TEntity> CrearAsync(TEntity entity)
        {
            return await _repository.Add(entity);
        }

        public virtual async Task<TEntity> ObtenerAsync(int id)
        {
            return await _repository.Get(id);
        }

        public virtual async Task<List<TEntity>> ObtenerTodosAsync()
        {
            return await _repository.GetAll();
        }

        public virtual async Task<TEntity> ActualizarAsync(TEntity entity)
        {
            return await _repository.Update(entity);
        }

        public virtual async Task<TEntity> EliminarAsync(int id)
        {
            return await _repository.Delete(id);
        }
    }
}