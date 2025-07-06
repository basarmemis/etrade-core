using etrade_core.domain.Entities.Base;

namespace etrade_core.application.IRepositories
{
    public interface ICompositeKeyRepository<TEntity> : ICompositeKeyReadRepository<TEntity>, ICompositeKeyWriteRepository<TEntity>
        where TEntity : class, ISoftDeletableEntity
    {
    }
} 