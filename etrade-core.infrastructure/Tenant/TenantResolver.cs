using etrade_core.application.Common.Base;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace etrade_core.infrastructure.Tenant
{
    /// <summary>
    /// Tenant resolver implementation that resolves tenant from multiple sources
    /// Priority: Header > Subdomain > Claim > Default
    /// </summary>
    public class TenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _defaultTenantId;
        private const string TenantIdHeader = "X-TenantId";
        private const string TenantIdClaim = "TenantId";

        public TenantResolver(IHttpContextAccessor httpContextAccessor, string defaultTenantId = "default")
        {
            _httpContextAccessor = httpContextAccessor;
            _defaultTenantId = defaultTenantId;
        }

        public async Task<string?> ResolveTenantIdAsync()
        {
            var tenantId = GetCurrentTenantId();
            return await Task.FromResult(tenantId);
        }

        public string? GetCurrentTenantId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return _defaultTenantId;

            // 1. Try to get from header
            var tenantId = GetTenantIdFromHeader(httpContext);
            if (!string.IsNullOrEmpty(tenantId))
                return tenantId;

            // 2. Try to get from subdomain
            tenantId = GetTenantIdFromSubdomain(httpContext);
            if (!string.IsNullOrEmpty(tenantId))
                return tenantId;

            // 3. Try to get from claims
            tenantId = GetTenantIdFromClaims(httpContext);
            if (!string.IsNullOrEmpty(tenantId))
                return tenantId;

            // 4. Return default tenant
            return _defaultTenantId;
        }

        public void SetCurrentTenantId(string tenantId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Set in items for the current request
                httpContext.Items["CurrentTenantId"] = tenantId;
            }
        }

        private string? GetTenantIdFromHeader(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.TryGetValue(TenantIdHeader, out var headerValue))
            {
                return headerValue.FirstOrDefault();
            }
            return null;
        }

        private string? GetTenantIdFromSubdomain(HttpContext httpContext)
        {
            var host = httpContext.Request.Host.Value;
            if (string.IsNullOrEmpty(host))
                return null;

            // Extract subdomain (e.g., company1.localhost:5000 -> company1)
            var parts = host.Split('.');
            if (parts.Length > 1)
            {
                var subdomain = parts[0];
                // Skip common subdomains like 'www', 'api', etc.
                if (!IsCommonSubdomain(subdomain))
                {
                    return subdomain;
                }
            }

            return null;
        }

        private string? GetTenantIdFromClaims(HttpContext httpContext)
        {
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = httpContext.User.FindFirst(TenantIdClaim);
                return tenantClaim?.Value;
            }
            return null;
        }

        private bool IsCommonSubdomain(string subdomain)
        {
            var commonSubdomains = new[] { "www", "api", "admin", "app", "mobile", "web" };
            return commonSubdomains.Contains(subdomain.ToLowerInvariant());
        }
    }
} 