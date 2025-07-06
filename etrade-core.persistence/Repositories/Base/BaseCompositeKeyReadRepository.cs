using System.Linq.Expressions;
using etrade_core.application.IRepositories;
using etrade_core.domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories.Base
{
    public abstract class BaseCompositeKeyReadRepository<TEntity, TContext> : ICompositeKeyReadRepository<TEntity>
        where TEntity : class, ISoftDeletableEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseCompositeKeyReadRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> GetQueryable(bool includeDeleted = false)
        {
            var query = _dbSet.AsQueryable();
            
            if (!includeDeleted)
            {
                query = query.Where(e => !e.IsDeleted);
            }
            
            return query;
        }

        // ICompositeKeyReadRepository implementation
        public virtual async Task<TEntity?> GetByCompositeKeyAsync(object[] keys)
        {
            return await GetByCompositeKeyAsync(keys, false);
        }

        public virtual async Task<TEntity?> GetByCompositeKeyAsync(object[] keys, bool includeDeleted = false)
        {
            // This method needs to be implemented by derived classes
            // as the composite key structure is specific to each entity
            await Task.CompletedTask;
            throw new NotImplementedException("GetByCompositeKeyAsync must be implemented by derived classes");
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await GetAllAsync(false);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAsync(predicate, false);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetFirstAsync(predicate, false);
        }

        public virtual async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetSingleAsync(predicate, false);
        }

        public virtual async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).SingleOrDefaultAsync(predicate);
        }

        public virtual async Task<int> CountAsync()
        {
            return await GetQueryable().CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await CountAsync(predicate, false);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).CountAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await AnyAsync(predicate, false);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).AnyAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(object[] keys)
        {
            return await ExistsAsync(keys, false);
        }

        public virtual async Task<bool> ExistsAsync(object[] keys, bool includeDeleted)
        {
            var entity = await GetByCompositeKeyAsync(keys, includeDeleted);
            return entity != null;
        }
    }
} 