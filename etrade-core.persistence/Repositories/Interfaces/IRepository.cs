using etrade_core.domain.Entities.Base;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>, IWriteRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>, ISoftDeletableEntity
        where TKey : IEquatable<TKey>
    {
    }
} 