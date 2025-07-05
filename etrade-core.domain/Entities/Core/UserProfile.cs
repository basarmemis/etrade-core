using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    public class UserProfile : BaseEntityWithoutAudit<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Foreign key for one-to-one relationship
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
} 