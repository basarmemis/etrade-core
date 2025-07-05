using etrade_core.domain.Entities.Core;

namespace etrade_core.persistence.Repositories.Interfaces
{
    public interface IUserProfileRepository : IRepository<UserProfile, int>
    {
        Task<UserProfile?> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserProfile>> GetByPhoneNumberAsync(string phoneNumber);
    }
} 