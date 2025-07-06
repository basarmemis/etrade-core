using etrade_core.application.IRepositories;
using etrade_core.domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories.Base
{
    public abstract class BaseWriteRepository<TEntity, TKey, TContext> : IWriteRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseWriteRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;
            
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();
            var now = DateTime.UtcNow;
            
            foreach (var entity in entityList)
            {
                entity.CreatedDate = now;
                entity.UpdatedDate = now;
            }
            
            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        public virtual void Update(TEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            var now = DateTime.UtcNow;
            var entityList = entities.ToList();
            
            foreach (var entity in entityList)
            {
                entity.UpdatedDate = now;
            }
            
            _dbSet.UpdateRange(entityList);
        }

        public virtual void Delete(TEntity entity)
        {
            // Default to soft delete
            SoftDelete(entity);
        }

        public virtual void Delete(TKey id)
        {
            // Default to soft delete
            SoftDelete(id);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities)
        {
            // Default to soft delete
            SoftDeleteRange(entities);
        }

        public virtual void DeleteRange(IEnumerable<TKey> ids)
        {
            // Default to soft delete
            SoftDeleteRange(ids);
        }

        public virtual void SoftDelete(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public virtual void SoftDelete(TKey id)
        {
            var entity = _dbSet.Find(id);
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
                entity.UpdatedDate = now;
            }
            
            _dbSet.UpdateRange(entityList);
        }

        public virtual void SoftDeleteRange(IEnumerable<TKey> ids)
        {
            var entities = _dbSet.Where(e => ids.Contains(e.Id)).ToList();
            SoftDeleteRange(entities);
        }

        public virtual void HardDelete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void HardDelete(TKey id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual void HardDeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void HardDeleteRange(IEnumerable<TKey> ids)
        {
            var entities = _dbSet.Where(e => ids.Contains(e.Id)).ToList();
            _dbSet.RemoveRange(entities);
        }
    }
} 