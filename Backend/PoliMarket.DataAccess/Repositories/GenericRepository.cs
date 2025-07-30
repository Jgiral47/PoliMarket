using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PoliMarket.DataAccess.Context;
using PoliMarket.DataAccess.Contracts;
using System.Linq.Expressions;

namespace PoliMarket.DataAccess.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        #endregion

        public GenericRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            context.Set<TEntity>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Delete(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            var entity = await context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity!;
            }

            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Get(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            return await context.Set<TEntity>().FindAsync(id) ?? null!;
        }

        public async Task<List<TEntity>> GetAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            IQueryable<TEntity> query = context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(property =>
            {
                query = query.Include(property);
            });

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync().ConfigureAwait(false);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        private static void ValidateRangeEntities(IEnumerable<TEntity> entities)
        {
            if (!entities?.Any() ?? true)
            {
                throw new ArgumentNullException(nameof(entities), "no se envió una lista de entidades a insertar");
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            ValidateRangeEntities(entities);
            try
            {
                context.Set<TEntity>().RemoveRange(entities);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync(string query)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            return await context.Set<TEntity>().FromSqlRaw(query).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync(string query, params object[] parameters)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PoliMarketDbContext>();
            return await context.Set<TEntity>().FromSqlRaw(query, parameters).ToListAsync().ConfigureAwait(false);
        }
    }
}