using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.Entities.Core
{
    /// <summary>
    /// ApplicationUser ile domain entity'leri arasında köprü görevi gören entity
    /// Sadece Identity User'ın Id'sini tutar ve domain entity'leri ile ilişki kurar
    /// </summary>
    public class UserProfile : BaseEntity<long>
    {
        // Identity User'ın Id'si - köprü görevi görür
        public long UserId { get; set; }
        
        // Navigation properties - domain entity'leri ile ilişkiler
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        // public ICollection<Basket> Baskets { get; set; } = new List<Basket>(); // Gelecekte eklenebilir
        // public ICollection<WishList> WishLists { get; set; } = new List<WishList>(); // Gelecekte eklenebilir
    }
} 