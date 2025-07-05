using Microsoft.AspNetCore.Identity;

namespace etrade_core.persistence.Identity
{
    public class ApplicationUser : IdentityUser<long>
    {
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        // Ekstra alanlar ekleyebilirsin
    }
} 