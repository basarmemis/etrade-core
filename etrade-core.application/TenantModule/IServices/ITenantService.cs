using etrade_core.domain.TenantModule.Entities;

namespace etrade_core.application.TenantModule.IServices
{
    /// <summary>
    /// Service interface for tenant management operations
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Gets all active tenants
        /// </summary>
        /// <returns>List of active tenants</returns>
        Task<IEnumerable<Tenant>> GetActiveTenantsAsync();

        /// <summary>
        /// Gets tenant by identifier
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>Tenant information</returns>
        Task<Tenant?> GetTenantByIdAsync(string tenantId);

        /// <summary>
        /// Creates a new tenant
        /// </summary>
        /// <param name="tenant">Tenant information</param>
        /// <returns>Created tenant</returns>
        Task<Tenant> CreateTenantAsync(Tenant tenant);

        /// <summary>
        /// Updates an existing tenant
        /// </summary>
        /// <param name="tenant">Updated tenant information</param>
        /// <returns>Updated tenant</returns>
        Task<Tenant> UpdateTenantAsync(Tenant tenant);

        /// <summary>
        /// Deactivates a tenant
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>True if successful</returns>
        Task<bool> DeactivateTenantAsync(string tenantId);

        /// <summary>
        /// Activates a tenant
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>True if successful</returns>
        Task<bool> ActivateTenantAsync(string tenantId);

        /// <summary>
        /// Checks if tenant exists and is active
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>True if tenant exists and is active</returns>
        Task<bool> IsTenantActiveAsync(string tenantId);
    }
} 