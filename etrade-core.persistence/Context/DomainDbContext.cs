using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.UserModule.Entities;
using etrade_core.persistence.Identity;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Context
{
    /// <summary>
    /// Domain entity'leri için ayrı DbContext
    /// Identity tabloları ApplicationDbContext'te, domain tabloları burada
    /// </summary>
    public class DomainDbContext : DbContext
    {
        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }

        // Business logic entity'leri
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UserProfile entity configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");
                entity.HasKey(e => e.Id);
                
                // Id property'sini long olarak tanımla
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // One-to-one relationship with Identity User (sadece Id referansı)
                entity.HasOne<ApplicationUser>()
                      .WithOne()
                      .HasForeignKey<UserProfile>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // One-to-many relationship with Orders
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.UserProfile)
                      .HasForeignKey(o => o.UserProfileId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Product entity configuration
            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Order entity configuration
            builder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                // Relationship with UserProfile (köprü entity)
                entity.HasOne(e => e.UserProfile)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(e => e.UserProfileId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // OrderItem entity configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                
                // Relationships
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
} 