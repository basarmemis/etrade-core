namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Unit of Work pattern interface - Transaction yönetimi ve repository koordinasyonu için
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repository properties
        IUserProfileRepository UserProfiles { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        
        // Generic Product System Repository Properties
        ICategoryRepository Categories { get; }
        IProductAttributeRepository ProductAttributes { get; }
        IProductTemplateRepository ProductTemplates { get; }
        IProductImageRepository ProductImages { get; }

        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // Save changes
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        
        // Transaction execution helpers
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
        
        // Transaction state
        bool HasActiveTransaction { get; }
        bool IsDisposed { get; }
    }
} 