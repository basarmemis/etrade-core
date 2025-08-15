namespace etrade_core.application.Common.Base
{
    /// <summary>
    /// Interface for managing tenant context information
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// Current tenant identifier
        /// </summary>
        string? CurrentTenantId { get; }

        /// <summary>
        /// Whether multi-tenancy is enabled
        /// </summary>
        bool IsMultiTenant { get; }

        /// <summary>
        /// Sets the current tenant for the current request
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        void SetCurrentTenant(string tenantId);

        /// <summary>
        /// Clears the current tenant context
        /// </summary>
        void ClearCurrentTenant();
    }
} 