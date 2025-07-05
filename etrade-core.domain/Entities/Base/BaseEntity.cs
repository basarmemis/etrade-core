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

    // Generic BaseEntity for different ID types
    public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletableEntity
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; } = default!;

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
    public abstract class BaseEntityWithoutAudit<TKey> : IEntity<TKey>, ISoftDeletableEntity
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; } = default!;

        public bool IsDeleted { get; set; } = false;
    }

    // Default BaseEntityWithoutAudit with int ID
    public abstract class BaseEntityWithoutAudit : BaseEntityWithoutAudit<int>
    {
    }

    // BaseEntity for composite keys with audit
    public abstract class BaseEntityWithCompositeKey : IAuditableEntity, ISoftDeletableEntity
    {
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        protected BaseEntityWithCompositeKey()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }

    // BaseEntity for composite keys without audit
    public abstract class BaseEntityWithCompositeKeyWithoutAudit : ISoftDeletableEntity
    {
        public bool IsDeleted { get; set; } = false;
    }
}