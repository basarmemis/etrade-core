using etrade_core.application.IRepositories;
using etrade_core.domain.Entities.Core;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Repositories
{
    public class UserProfileRepository : BaseRepository<UserProfile, long, DomainDbContext>, IUserProfileRepository
    {
        public UserProfileRepository(DomainDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Identity User Id'sine göre UserProfile'ı bulur
        /// </summary>
        public async Task<UserProfile?> GetByUserIdAsync(long userId)
        {
            return await GetFirstAsync(p => p.UserId == userId);
        }

        /// <summary>
        /// Identity User Id'sine göre UserProfile'ı Orders ile birlikte getirir
        /// </summary>
        public async Task<UserProfile?> GetByUserIdWithOrdersAsync(long userId)
        {
            // Şimdilik basit implementasyon - gelecekte Include ile geliştirilebilir
            return await GetFirstAsync(p => p.UserId == userId);
        }
    }
} 