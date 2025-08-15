using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.UserModule.Entities;
using etrade_core.domain.CategoryModule.Entities;
using etrade_core.domain.TenantModule.Entities;
using etrade_core.persistence.Identity;
using etrade_core.application.Common.Base;
using etrade_core.domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Context
{
    /// <summary>
    /// Domain entity'leri için ayrı DbContext
    /// Identity tabloları ApplicationDbContext'te, domain tabloları burada
    /// </summary>
    public class DomainDbContext : DbContext
    {
        private readonly ITenantResolver? _tenantResolver;

        public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
        {
        }

        public DomainDbContext(DbContextOptions<DomainDbContext> options, ITenantResolver? tenantResolver) : base(options)
        {
            _tenantResolver = tenantResolver;
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

        // Tenant entity
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tenant entity configuration
            builder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants");
                entity.HasKey(e => e.Id);
                
                // Unique constraint on TenantId
                entity.HasIndex(e => e.TenantId).IsUnique();
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // UserProfile entity configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfiles");
                entity.HasKey(e => e.Id);
                
                // Id property'sini long olarak tanımla
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // Order entity configuration
            builder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
                // Status property - int olarak saklanacak
                entity.Property(e => e.Status).HasConversion<int>();
                
                // OfferingTypes property - Flags enum için
                entity.Property(e => e.OfferingTypes).HasConversion<int>();
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // OrderItem entity configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
                // OfferingType property - Flags enum için
                entity.Property(e => e.OfferingType).HasConversion<int>();
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // Category entity configuration
            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // Product entity configuration (updated)
            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // ProductAttribute entity configuration
            builder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("ProductAttributes");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // ProductImage entity configuration
            builder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("ProductImages");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // ProductPriceHistory entity configuration
            builder.Entity<ProductPriceHistory>(entity =>
            {
                entity.ToTable("ProductPriceHistories");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // ProductTemplate entity configuration
            builder.Entity<ProductTemplate>(entity =>
            {
                entity.ToTable("ProductTemplates");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // ProductTemplateAttribute entity configuration
            builder.Entity<ProductTemplateAttribute>(entity =>
            {
                entity.ToTable("ProductTemplateAttributes");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
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
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });

            // OrderItemAttribute entity configuration
            builder.Entity<OrderItemAttribute>(entity =>
            {
                entity.ToTable("OrderItemAttributes");
                entity.HasKey(e => e.Id);
                
                // TenantId property
                entity.Property(e => e.TenantId).IsRequired().HasMaxLength(50);
                
                // Relationship with OrderItem
                entity.HasOne(e => e.OrderItem)
                      .WithMany(oi => oi.Attributes)
                      .HasForeignKey(e => e.OrderItemId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Audit fields
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                // Soft delete filter
                entity.HasQueryFilter(e => !e.IsDeleted);
                
                // Tenant filter
                entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            SetTenantIdOnEntities();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetTenantIdOnEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetTenantIdOnEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetTenantIdOnEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetTenantIdOnEntities()
        {
            var currentTenantId = GetCurrentTenantId();
            if (string.IsNullOrEmpty(currentTenantId))
                return;

            var entities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity)
                .Select(e => e.Entity as ITenantEntity);

            foreach (var entity in entities)
            {
                if (entity != null && string.IsNullOrEmpty(entity.TenantId))
                {
                    entity.TenantId = currentTenantId;
                }
            }
        }

        private string? GetCurrentTenantId()
        {
            if (_tenantResolver != null)
            {
                return _tenantResolver.GetCurrentTenantId();
            }

            // Fallback to default tenant if resolver is not available
            return "default";
        }
    }
} 