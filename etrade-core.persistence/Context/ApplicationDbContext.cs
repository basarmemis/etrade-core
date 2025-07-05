using etrade_core.persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace etrade_core.persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, IdentityUserClaim<long>, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("AspNetRoles");
                entity.Property(e => e.CreatedDate).IsRequired();
                entity.Property(e => e.UpdatedDate);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            builder.Entity<ApplicationUserRole>(entity =>
            {
                entity.ToTable("AspNetUserRoles");
            });

            builder.Entity<IdentityUserClaim<long>>(entity =>
            {
                entity.ToTable("AspNetUserClaims");
            });

            builder.Entity<ApplicationUserLogin>(entity =>
            {
                entity.ToTable("AspNetUserLogins");
            });

            builder.Entity<ApplicationUserToken>(entity =>
            {
                entity.ToTable("AspNetUserTokens");
            });

            builder.Entity<ApplicationRoleClaim>(entity =>
            {
                entity.ToTable("AspNetRoleClaims");
            });
        }
    }

    // Identity custom types for long key
    public class ApplicationUserRole : IdentityUserRole<long> { }
    public class ApplicationUserLogin : IdentityUserLogin<long> { }
    public class ApplicationUserToken : IdentityUserToken<long> { }
    public class ApplicationRoleClaim : IdentityRoleClaim<long> { }
} 