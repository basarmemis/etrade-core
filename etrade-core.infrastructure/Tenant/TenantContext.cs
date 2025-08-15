using etrade_core.application.Common.Base;
using Microsoft.AspNetCore.Http;

namespace etrade_core.infrastructure.Tenant
{
    /// <summary>
    /// Tenant context implementation for managing tenant information
    /// </summary>
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CurrentTenantKey = "CurrentTenantId";
        private const string IsMultiTenantKey = "IsMultiTenant";

        public TenantContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? CurrentTenantId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Items.TryGetValue(CurrentTenantKey, out var tenantId) == true)
                {
                    return tenantId as string;
                }
                return null;
            }
        }

        public bool IsMultiTenant
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Items.TryGetValue(IsMultiTenantKey, out var isMultiTenant) == true)
                {
                    return isMultiTenant is bool && (bool)isMultiTenant;
                }
                return false;
            }
        }

        public void SetCurrentTenant(string tenantId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Items[CurrentTenantKey] = tenantId;
                httpContext.Items[IsMultiTenantKey] = true;
            }
        }

        public void ClearCurrentTenant()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Items.Remove(CurrentTenantKey);
                httpContext.Items.Remove(IsMultiTenantKey);
            }
        }
    }
} 