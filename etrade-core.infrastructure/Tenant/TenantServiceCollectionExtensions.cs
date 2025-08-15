using etrade_core.application.Common.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace etrade_core.infrastructure.Tenant
{
    /// <summary>
    /// Extension methods for registering tenant services
    /// </summary>
    public static class TenantServiceCollectionExtensions
    {
        /// <summary>
        /// Adds tenant services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultTenantId">Default tenant identifier</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantServices(this IServiceCollection services, string defaultTenantId = "default")
        {
            // Register HttpContextAccessor if not already registered
            services.AddHttpContextAccessor();

            // Register tenant services
            services.AddScoped<ITenantResolver>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new TenantResolver(httpContextAccessor, defaultTenantId);
            });

            services.AddScoped<ITenantContext>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new TenantContext(httpContextAccessor);
            });

            return services;
        }

        /// <summary>
        /// Adds tenant services with custom tenant resolver
        /// </summary>
        /// <typeparam name="TTenantResolver">Custom tenant resolver type</typeparam>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantServices<TTenantResolver>(this IServiceCollection services)
            where TTenantResolver : class, ITenantResolver
        {
            // Register HttpContextAccessor if not already registered
            services.AddHttpContextAccessor();

            // Register tenant services
            services.AddScoped<ITenantResolver, TTenantResolver>();
            services.AddScoped<ITenantContext>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new TenantContext(httpContextAccessor);
            });

            return services;
        }
    }
} 