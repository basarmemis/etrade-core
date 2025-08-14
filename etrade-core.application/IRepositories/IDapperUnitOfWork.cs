using System.Data;
using etrade_core.domain.UserModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Dapper için Unit of Work interface
    /// EF Core ile birlikte kullanılabilir
    /// </summary>
    public interface IDapperUnitOfWork : IDisposable
    {
        // Dapper Repository Properties
        IDapperRepository<UserProfile, long> UserProfiles { get; }
        IDapperRepository<Product, long> Products { get; }
        IDapperRepository<Order, long> Orders { get; }
        IDapperRepository<OrderItem, long> OrderItems { get; }
        
        // Generic Product System Repository Properties
        IDapperRepository<Category, long> Categories { get; }
        IDapperRepository<ProductAttribute, long> ProductAttributes { get; }
        IDapperRepository<ProductTemplate, long> ProductTemplates { get; }
        IDapperRepository<ProductImage, long> ProductImages { get; }

        // Transaction management
        Task<IDbTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // Transaction execution helpers
        Task<T> ExecuteInTransactionAsync<T>(Func<IDbTransaction, Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<IDbTransaction, Task> operation);
        
        // Transaction state
        bool HasActiveTransaction { get; }
        bool IsDisposed { get; }
    }
} 