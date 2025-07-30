using System.Linq.Expressions;

namespace PoliMarket.DataAccess.Contracts
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Add(TEntity entity);
        Task<TEntity> Delete(int id);
        Task<TEntity> Get(int id);
        Task<List<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        Task<TEntity> Update(TEntity entity);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync(string query);
        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync(string query, params object[] parameters);
    }
}