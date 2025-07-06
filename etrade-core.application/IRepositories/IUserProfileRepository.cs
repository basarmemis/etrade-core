using etrade_core.domain.UserModule.Entities;

namespace etrade_core.application.IRepositories
{
    public interface IUserProfileRepository : IRepository<UserProfile, long>
    {
        Task<UserProfile?> GetByUserIdAsync(long userId);
        Task<UserProfile?> GetByUserIdWithOrdersAsync(long userId);
    }
} 