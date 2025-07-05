using etrade_core.domain.Entities.Core;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile, long>
    {
        Task<UserProfile?> GetByUserIdAsync(long userId);
        Task<UserProfile?> GetByUserIdWithOrdersAsync(long userId);
    }
} 