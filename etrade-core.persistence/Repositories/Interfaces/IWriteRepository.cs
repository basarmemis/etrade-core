using etrade_core.domain.Entities.Base;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IWriteRepository<TEntity, TKey> 
        where TEntity : class, ISoftDeletableEntity
        where TKey : IEquatable<TKey>
    {
        // Add operations
        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
        
        // Update operations
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        
        // Delete operations
        void Delete(TEntity entity);
        void Delete(TKey id);
        void DeleteRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<TKey> ids);
        
        // Soft delete operations
        void SoftDelete(TEntity entity);
        void SoftDelete(TKey id);
        void SoftDeleteRange(IEnumerable<TEntity> entities);
        void SoftDeleteRange(IEnumerable<TKey> ids);
        
        // Hard delete operations (permanent)
        void HardDelete(TEntity entity);
        void HardDelete(TKey id);
        void HardDeleteRange(IEnumerable<TEntity> entities);
        void HardDeleteRange(IEnumerable<TKey> ids);
    }
} 