using etrade_core.application.TenantModule.IServices;
using etrade_core.domain.TenantModule.Entities;
using etrade_core.application.IRepositories;
using etrade_core.application.Common.Base;

namespace etrade_core.application.TenantModule.Services
{
    /// <summary>
    /// Service implementation for tenant management operations
    /// </summary>
    public class TenantService : ITenantService
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantResolver _tenantResolver;
        private readonly ITenantContext _tenantContext;

        public TenantService(
            IRepository<Tenant, int> tenantRepository,
            IUnitOfWork unitOfWork,
            ITenantResolver tenantResolver,
            ITenantContext tenantContext)
        {
            _tenantRepository = tenantRepository;
            _unitOfWork = unitOfWork;
            _tenantResolver = tenantResolver;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync()
        {
            // This method should be accessible from any tenant context
            // as it's used for tenant management
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.Where(t => t.IsActive && !t.IsDeleted);
        }

        public async Task<Tenant?> GetTenantByIdAsync(string tenantId)
        {
            // This method should be accessible from any tenant context
            // as it's used for tenant management
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.FirstOrDefault(t => t.TenantId == tenantId && !t.IsDeleted);
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            // Validate tenant ID uniqueness
            var existingTenant = await GetTenantByIdAsync(tenant.TenantId);
            if (existingTenant != null)
            {
                throw new InvalidOperationException($"Tenant with ID '{tenant.TenantId}' already exists.");
            }

            // Set creation date
            tenant.CreatedDate = DateTime.UtcNow;
            tenant.IsActive = true;
            tenant.IsDeleted = false;

            // Create the tenant
            var createdTenant = await _tenantRepository.AddAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            return createdTenant;
        }

        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            // Validate tenant exists
            var existingTenant = await GetTenantByIdAsync(tenant.TenantId);
            if (existingTenant == null)
            {
                throw new InvalidOperationException($"Tenant with ID '{tenant.TenantId}' not found.");
            }

            // Update fields
            existingTenant.Name = tenant.Name;
            existingTenant.Description = tenant.Description;
            existingTenant.ConnectionString = tenant.ConnectionString;
            existingTenant.Configuration = tenant.Configuration;
            existingTenant.UpdatedDate = DateTime.UtcNow;

            // Update the tenant
            _tenantRepository.Update(existingTenant);
            await _unitOfWork.SaveChangesAsync();

            return existingTenant;
        }

        public async Task<bool> DeactivateTenantAsync(string tenantId)
        {
            var tenant = await GetTenantByIdAsync(tenantId);
            if (tenant == null)
                return false;

            tenant.IsActive = false;
            tenant.UpdatedDate = DateTime.UtcNow;

            _tenantRepository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ActivateTenantAsync(string tenantId)
        {
            var tenant = await GetTenantByIdAsync(tenantId);
            if (tenant == null)
                return false;

            tenant.IsActive = true;
            tenant.UpdatedDate = DateTime.UtcNow;

            _tenantRepository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsTenantActiveAsync(string tenantId)
        {
            var tenant = await GetTenantByIdAsync(tenantId);
            return tenant?.IsActive == true;
        }
    }
} 