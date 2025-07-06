using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.UserModule.Entities;
using etrade_core.domain.CategoryModule.Entities;
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
        
        // Generic Product System
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }
        public DbSet<ProductTemplate> ProductTemplates { get; set; }
        public DbSet<ProductTemplateAttribute> ProductTemplateAttributes { get; set; }

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

            // Category entity configuration
            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                
                // Self-referencing relationship for hierarchical categories
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.SubCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Product entity configuration (updated)
            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                
                // Relationships
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.ProductTemplate)
                      .WithMany(t => t.Products)
                      .HasForeignKey(e => e.ProductTemplateId)
                      .OnDelete(DeleteBehavior.SetNull);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProductAttribute entity configuration
            builder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("ProductAttributes");
                entity.HasKey(e => e.Id);
                
                // Relationship with Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Attributes)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Index for better performance
                entity.HasIndex(e => new { e.ProductId, e.AttributeKey }).IsUnique();
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProductImage entity configuration
            builder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages");
                entity.HasKey(e => e.Id);
                
                // Relationship with Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Images)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProductPriceHistory entity configuration
            builder.Entity<ProductPriceHistory>(entity =>
            {
                entity.ToTable("ProductPriceHistories");
                entity.HasKey(e => e.Id);
                
                // Relationship with Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.PriceHistory)
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProductTemplate entity configuration
            builder.Entity<ProductTemplate>(entity =>
            {
                entity.ToTable("ProductTemplates");
                entity.HasKey(e => e.Id);
                
                // Relationship with Category
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // ProductTemplateAttribute entity configuration
            builder.Entity<ProductTemplateAttribute>(entity =>
            {
                entity.ToTable("ProductTemplateAttributes");
                entity.HasKey(e => e.Id);
                
                // Relationship with ProductTemplate
                entity.HasOne(e => e.ProductTemplate)
                      .WithMany(t => t.TemplateAttributes)
                      .HasForeignKey(e => e.ProductTemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Index for better performance
                entity.HasIndex(e => new { e.ProductTemplateId, e.AttributeKey }).IsUnique();
                
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