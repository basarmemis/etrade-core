using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories;
using etrade_core.persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace etrade_core.persistence.Extensions
{
    /// <summary>
    /// Service Collection için extension methods - DI registration
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Persistence katmanı için gerekli servisleri DI container'a kaydeder
        /// </summary>
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
        {
            // DbContext registration
            services.AddDbContext<DomainDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Repository registrations
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            // Unit of Work registration
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
} 