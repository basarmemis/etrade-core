namespace etrade_core.application.Common.Base
{
    /// <summary>
    /// Interface for resolving tenant information from different sources
    /// </summary>
    public interface ITenantResolver
    {
        /// <summary>
        /// Resolves the current tenant identifier
        /// </summary>
        /// <returns>Current tenant identifier or null if not found</returns>
        Task<string?> ResolveTenantIdAsync();

        /// <summary>
        /// Gets the current tenant identifier synchronously
        /// </summary>
        /// <returns>Current tenant identifier or null if not found</returns>
        string? GetCurrentTenantId();

        /// <summary>
        /// Sets the current tenant identifier for the current request context
        /// </summary>
        /// <param name="tenantId">Tenant identifier to set</param>
        void SetCurrentTenantId(string tenantId);
    }
} 