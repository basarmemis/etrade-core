using etrade_core.application.Common.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace etrade_core.infrastructure.Tenant
{
    /// <summary>
    /// Middleware for automatically resolving and setting tenant information
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITenantResolver _tenantResolver;
        private readonly ITenantContext _tenantContext;

        public TenantMiddleware(
            RequestDelegate next,
            ITenantResolver tenantResolver,
            ITenantContext tenantContext)
        {
            _next = next;
            _tenantResolver = tenantResolver;
            _tenantContext = tenantContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Resolve tenant from the request
                var tenantId = await _tenantResolver.ResolveTenantIdAsync();
                
                if (!string.IsNullOrEmpty(tenantId))
                {
                    // Set the tenant in the context
                    _tenantContext.SetCurrentTenant(tenantId);
                    
                    // Also set it in the resolver for backward compatibility
                    _tenantResolver.SetCurrentTenantId(tenantId);
                }

                // Continue with the request pipeline
                await _next(context);
            }
            finally
            {
                // Clean up tenant context after the request
                _tenantContext.ClearCurrentTenant();
            }
        }
    }

    /// <summary>
    /// Extension methods for registering the tenant middleware
    /// </summary>
    public static class TenantMiddlewareExtensions
    {
        /// <summary>
        /// Adds tenant middleware to the application pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TenantMiddleware>();
        }
    }
} 