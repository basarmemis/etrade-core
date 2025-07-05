using etrade_core.domain.Entities.Core;
using etrade_core.persistence.Repositories.Base;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class UserProfileRepository : BaseRepository<UserProfile, int, DbContext>, IUserProfileRepository
    {
        public UserProfileRepository(DbContext context) : base(context)
        {
        }

        // Custom methods specific to UserProfile
        public async Task<UserProfile?> GetByUserIdAsync(int userId)
        {
            return await GetFirstAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await GetAsync(p => p.PhoneNumber == phoneNumber);
        }
    }
} 