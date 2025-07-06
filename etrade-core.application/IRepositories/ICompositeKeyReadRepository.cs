using System.Linq.Expressions;
using etrade_core.domain.Entities.Base;

namespace etrade_core.application.IRepositories
{
    public interface ICompositeKeyReadRepository<TEntity> 
        where TEntity : class, ISoftDeletableEntity
    {
        // Get by composite key
        Task<TEntity?> GetByCompositeKeyAsync(object[] keys);
        Task<TEntity?> GetByCompositeKeyAsync(object[] keys, bool includeDeleted = false);
        
        // Get all
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(bool includeDeleted);
        
        // Get with predicate
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
        
        // Get first
        Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
        
        // Get single
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
        
        // Count
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
        
        // Any
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted);
        
        // Exists
        Task<bool> ExistsAsync(object[] keys);
        Task<bool> ExistsAsync(object[] keys, bool includeDeleted);
    }
}