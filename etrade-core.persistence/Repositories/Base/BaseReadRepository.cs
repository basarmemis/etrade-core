using System.Linq.Expressions;
using etrade_core.domain.Entities.Base;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories.Base
{
    public abstract class BaseReadRepository<TEntity, TKey, TContext> : IReadRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseReadRepository(TContext context)
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

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await GetByIdAsync(id, false);
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, bool includeDeleted = false)
        {
            return await GetQueryable(includeDeleted).FirstOrDefaultAsync(e => e.Id.Equals(id));
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

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            return await ExistsAsync(id, false);
        }

        public virtual async Task<bool> ExistsAsync(TKey id, bool includeDeleted)
        {
            return await GetQueryable(includeDeleted).AnyAsync(e => e.Id.Equals(id));
        }
    }
} 