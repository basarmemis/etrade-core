using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

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
            // Note: DbContext registration is handled in Program.cs to avoid conflicts
            // This method only registers repositories and unit of work

            // EF Core Repository registrations
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            
            // Generic Product System Repository registrations
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductTemplateRepository, ProductTemplateRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();

            // EF Core Unit of Work registration
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Dapper services registration
            services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));
            services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();

            return services;
        }
    }
} 