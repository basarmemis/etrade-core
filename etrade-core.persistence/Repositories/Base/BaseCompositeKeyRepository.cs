using System.Linq.Expressions;
using etrade_core.domain.Entities.Base;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories.Base
{
    public abstract class BaseCompositeKeyRepository<TEntity, TContext> : ICompositeKeyRepository<TEntity>
        where TEntity : class, ISoftDeletableEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseCompositeKeyRepository(TContext context)
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

        // ICompositeKeyWriteRepository implementation
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            // Set audit fields if entity implements IAuditableEntity
            if (entity is IAuditableEntity auditableEntity)
            {
                auditableEntity.CreatedDate = DateTime.UtcNow;
                auditableEntity.UpdatedDate = DateTime.UtcNow;
            }
            
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();
            var now = DateTime.UtcNow;
            
            foreach (var entity in entityList)
            {
                // Set audit fields if entity implements IAuditableEntity
                if (entity is IAuditableEntity auditableEntity)
                {
                    auditableEntity.CreatedDate = now;
                    auditableEntity.UpdatedDate = now;
                }
            }
            
            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        public virtual void Update(TEntity entity)
        {
            // Set audit fields if entity implements IAuditableEntity
            if (entity is IAuditableEntity auditableEntity)
            {
                auditableEntity.UpdatedDate = DateTime.UtcNow;
            }
            
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            var now = DateTime.UtcNow;
            var entityList = entities.ToList();
            
            foreach (var entity in entityList)
            {
                // Set audit fields if entity implements IAuditableEntity
                if (entity is IAuditableEntity auditableEntity)
                {
                    auditableEntity.UpdatedDate = now;
                }
            }
            
            _dbSet.UpdateRange(entityList);
        }

        public virtual void Delete(TEntity entity)
        {
            // Default to soft delete
            SoftDelete(entity);
        }

        public virtual void Delete(object[] keys)
        {
            // Default to soft delete
            SoftDelete(keys);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            // Default to soft delete
            SoftDeleteRange(entities);
        }

        public virtual void DeleteRange(IEnumerable<object[]> keys)
        {
            // Default to soft delete
            SoftDeleteRange(keys);
        }

        public virtual void SoftDelete(TEntity entity)
        {
            entity.IsDeleted = true;
            
            // Set audit fields if entity implements IAuditableEntity
            if (entity is IAuditableEntity auditableEntity)
            {
                auditableEntity.UpdatedDate = DateTime.UtcNow;
            }
            
            _dbSet.Update(entity);
        }

        public virtual void SoftDelete(object[] keys)
        {
            var entity = GetByCompositeKeyAsync(keys).Result;
            if (entity != null)
            {
                SoftDelete(entity);
            }
        }

        public virtual void SoftDeleteRange(IEnumerable<TEntity> entities)
        {
            var now = DateTime.UtcNow;
            var entityList = entities.ToList();
            
            foreach (var entity in entityList)
            {
                entity.IsDeleted = true;
                
                // Set audit fields if entity implements IAuditableEntity
                if (entity is IAuditableEntity auditableEntity)
                {
                    auditableEntity.UpdatedDate = now;
                }
            }
            
            _dbSet.UpdateRange(entityList);
        }

        public virtual void SoftDeleteRange(IEnumerable<object[]> keys)
        {
            var entities = new List<TEntity>();
            foreach (var key in keys)
            {
                var entity = GetByCompositeKeyAsync(key).Result;
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            SoftDeleteRange(entities);
        }

        public virtual void HardDelete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void HardDelete(object[] keys)
        {
            var entity = GetByCompositeKeyAsync(keys).Result;
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual void HardDeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void HardDeleteRange(IEnumerable<object[]> keys)
        {
            var entities = new List<TEntity>();
            foreach (var key in keys)
            {
                var entity = GetByCompositeKeyAsync(key).Result;
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            _dbSet.RemoveRange(entities);
        }
    }
} 