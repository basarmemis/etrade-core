using System.ComponentModel.DataAnnotations;

namespace etrade_core.domain.Entities.Base
{
    // Interface for entities with ID property
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    // Audit interface for entities that need tracking
    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
    }

    // Soft delete interface
    public interface ISoftDeletableEntity
    {
        bool IsDeleted { get; set; }
    }

    // Multi-tenant interface
    public interface ITenantEntity
    {
        string TenantId { get; set; }
    }

    // Generic BaseEntity for different ID types with tenant support
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletableEntity, ITenantEntity
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; } = default!;

        [Required]
        [MaxLength(50)]
        public string TenantId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }

    // Default BaseEntity with int ID for backward compatibility
    public abstract class BaseEntity : BaseEntity<int>
    {
    }

    // BaseEntity without audit for one-to-one relationships
    public abstract class BaseEntityWithoutAudit<TKey> : IEntity<TKey>, ISoftDeletableEntity, ITenantEntity
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; } = default!;

        [Required]
        [MaxLength(50)]
        public string TenantId { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
    }

    // Default BaseEntityWithoutAudit with int ID
    public abstract class BaseEntityWithoutAudit : BaseEntityWithoutAudit<int>
    {
    }

    // BaseEntity for composite keys with audit
    public abstract class BaseEntityWithCompositeKey : IAuditableEntity, ISoftDeletableEntity, ITenantEntity
    {
        [Required]
        [MaxLength(50)]
        public string TenantId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        protected BaseEntityWithCompositeKey()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }

    // BaseEntity for composite keys without audit
    public abstract class BaseEntityWithCompositeKeyWithoutAudit : ISoftDeletableEntity, ITenantEntity
    {
        [Required]
        [MaxLength(50)]
        public string TenantId { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;
    }
}