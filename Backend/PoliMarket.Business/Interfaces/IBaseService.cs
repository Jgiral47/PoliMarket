namespace PoliMarket.Business.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<TEntity> CrearAsync(TEntity entity);
        Task<TEntity> ObtenerAsync(int id);
        Task<List<TEntity>> ObtenerTodosAsync();
        Task<TEntity> ActualizarAsync(TEntity entity);
        Task<TEntity> EliminarAsync(int id);
    }
}