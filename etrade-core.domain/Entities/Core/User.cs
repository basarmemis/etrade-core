using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class User : BaseEntity<int>
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        // One-to-one relationship
        public UserProfile? Profile { get; set; }
    }
} 