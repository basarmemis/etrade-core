using etrade_core.domain.Entities.Base;

namespace etrade_core.domain.TenantModule.Entities
{
    /// <summary>
    /// Tenant entity for multi-tenancy support
    /// </summary>
    public class Tenant : BaseEntity
    {
        /// <summary>
        /// Unique tenant identifier (e.g., "company1", "company2")
        /// </summary>
        public new string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// Display name for the tenant
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the tenant
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Connection string for tenant-specific database (optional)
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Whether the tenant is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Tenant-specific configuration as JSON
        /// </summary>
        public string? Configuration { get; set; }
    }
} 