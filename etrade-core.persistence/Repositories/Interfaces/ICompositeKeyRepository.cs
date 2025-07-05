using etrade_core.domain.Entities.Base;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface ICompositeKeyRepository<TEntity> : ICompositeKeyReadRepository<TEntity>, ICompositeKeyWriteRepository<TEntity>
        where TEntity : class, ISoftDeletableEntity
    {
    }
} 