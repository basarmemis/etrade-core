using etrade_core.domain.Entities.Base;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories.Base
{
    public abstract class BaseCompositeKeyWriteRepository<TEntity, TContext> : ICompositeKeyWriteRepository<TEntity>
        where TEntity : class, ISoftDeletableEntity
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseCompositeKeyWriteRepository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
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
            // This method needs to be implemented by derived classes
            // as it requires access to the read repository for finding the entity
            throw new NotImplementedException("SoftDelete with keys must be implemented by derived classes");
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
            // This method needs to be implemented by derived classes
            // as it requires access to the read repository for finding entities
            throw new NotImplementedException("SoftDeleteRange with keys must be implemented by derived classes");
        }

        public virtual void HardDelete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void HardDelete(object[] keys)
        {
            // This method needs to be implemented by derived classes
            // as it requires access to the read repository for finding the entity
            throw new NotImplementedException("HardDelete with keys must be implemented by derived classes");
        }

        public virtual void HardDeleteRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual void HardDeleteRange(IEnumerable<object[]> keys)
        {
            // This method needs to be implemented by derived classes
            // as it requires access to the read repository for finding entities
            throw new NotImplementedException("HardDeleteRange with keys must be implemented by derived classes");
        }
    }
} 