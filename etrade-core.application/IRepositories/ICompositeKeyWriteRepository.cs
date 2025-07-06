using etrade_core.domain.Entities.Base;

namespace etrade_core.application.IRepositories
{
    public interface ICompositeKeyWriteRepository<TEntity> 
        where TEntity : class, ISoftDeletableEntity
    {
        // Add operations
        Task<TEntity> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
        
        // Update operations
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        
        // Delete operations
        void Delete(TEntity entity);
        void Delete(object[] keys);
        void DeleteRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<object[]> keys);
        
        // Soft delete operations
        void SoftDelete(TEntity entity);
        void SoftDelete(object[] keys);
        void SoftDeleteRange(IEnumerable<TEntity> entities);
        void SoftDeleteRange(IEnumerable<object[]> keys);
        
        // Hard delete operations (permanent)
        void HardDelete(TEntity entity);
        void HardDelete(object[] keys);
        void HardDeleteRange(IEnumerable<TEntity> entities);
        void HardDeleteRange(IEnumerable<object[]> keys);
    }
} 