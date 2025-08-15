using etrade_core.application.TenantModule.IServices;
using etrade_core.domain.TenantModule.Entities;
using Microsoft.AspNetCore.Mvc;

namespace etrade_core.api.Controllers
{
    /// <summary>
    /// Controller for tenant management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Gets all active tenants
        /// </summary>
        /// <returns>List of active tenants</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetActiveTenants()
        {
            try
            {
                var tenants = await _tenantService.GetActiveTenantsAsync();
                return Ok(tenants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets tenant by identifier
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>Tenant information</returns>
        [HttpGet("{tenantId}")]
        public async Task<ActionResult<Tenant>> GetTenantById(string tenantId)
        {
            try
            {
                var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
                if (tenant == null)
                    return NotFound($"Tenant with ID '{tenantId}' not found.");

                return Ok(tenant);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new tenant
        /// </summary>
        /// <param name="tenant">Tenant information</param>
        /// <returns>Created tenant</returns>
        [HttpPost]
        public async Task<ActionResult<Tenant>> CreateTenant([FromBody] Tenant tenant)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdTenant = await _tenantService.CreateTenantAsync(tenant);
                return CreatedAtAction(nameof(GetTenantById), new { tenantId = createdTenant.TenantId }, createdTenant);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing tenant
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="tenant">Updated tenant information</param>
        /// <returns>Updated tenant</returns>
        [HttpPut("{tenantId}")]
        public async Task<ActionResult<Tenant>> UpdateTenant(string tenantId, [FromBody] Tenant tenant)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (tenantId != tenant.TenantId)
                    return BadRequest("Tenant ID mismatch");

                var updatedTenant = await _tenantService.UpdateTenantAsync(tenant);
                return Ok(updatedTenant);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deactivates a tenant
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>Success status</returns>
        [HttpPost("{tenantId}/deactivate")]
        public async Task<ActionResult> DeactivateTenant(string tenantId)
        {
            try
            {
                var result = await _tenantService.DeactivateTenantAsync(tenantId);
                if (!result)
                    return NotFound($"Tenant with ID '{tenantId}' not found.");

                return Ok(new { message = "Tenant deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Activates a tenant
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>Success status</returns>
        [HttpPost("{tenantId}/activate")]
        public async Task<ActionResult> ActivateTenant(string tenantId)
        {
            try
            {
                var result = await _tenantService.ActivateTenantAsync(tenantId);
                if (!result)
                    return NotFound($"Tenant with ID '{tenantId}' not found.");

                return Ok(new { message = "Tenant activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if tenant is active
        /// </summary>
        /// <param name="tenantId">Tenant identifier</param>
        /// <returns>Active status</returns>
        [HttpGet("{tenantId}/status")]
        public async Task<ActionResult> GetTenantStatus(string tenantId)
        {
            try
            {
                var isActive = await _tenantService.IsTenantActiveAsync(tenantId);
                return Ok(new { tenantId, isActive });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 