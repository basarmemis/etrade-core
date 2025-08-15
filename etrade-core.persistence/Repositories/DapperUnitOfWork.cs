using System.Data;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using etrade_core.persistence.Repositories.Base;
using etrade_core.domain.UserModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.persistence.Repositories
{
    /// <summary>
    /// Dapper için Unit of Work implementasyonu
    /// EF Core ile birlikte kullanılabilir
    /// </summary>
    public class DapperUnitOfWork : IDapperUnitOfWork
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbTransaction? _currentTransaction;
        private bool _disposed;

        // Repository instances
        private readonly Lazy<IDapperRepository<UserProfile, long>> _userProfiles;
        private readonly Lazy<IDapperRepository<Product, long>> _products;
        private readonly Lazy<IDapperRepository<Order, long>> _orders;
        private readonly Lazy<IDapperRepository<OrderItem, long>> _orderItems;
        private readonly Lazy<IDapperRepository<Category, long>> _categories;
        private readonly Lazy<IDapperRepository<ProductAttribute, long>> _productAttributes;
        private readonly Lazy<IDapperRepository<ProductTemplate, long>> _productTemplates;
        private readonly Lazy<IDapperRepository<ProductImage, long>> _productImages;

        public DapperUnitOfWork(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

            // Lazy initialization of repositories
            _userProfiles = new Lazy<IDapperRepository<UserProfile, long>>(() => new DapperUserProfileRepository(connectionFactory));
            _products = new Lazy<IDapperRepository<Product, long>>(() => new DapperProductRepository(connectionFactory));
            _orders = new Lazy<IDapperRepository<Order, long>>(() => new DapperOrderRepository(connectionFactory));
            _orderItems = new Lazy<IDapperRepository<OrderItem, long>>(() => new DapperOrderItemRepository(connectionFactory));
            _categories = new Lazy<IDapperRepository<Category, long>>(() => new DapperCategoryRepository(connectionFactory));
            _productAttributes = new Lazy<IDapperRepository<ProductAttribute, long>>(() => new DapperProductAttributeRepository(connectionFactory));
            _productTemplates = new Lazy<IDapperRepository<ProductTemplate, long>>(() => new DapperProductTemplateRepository(connectionFactory));
            _productImages = new Lazy<IDapperRepository<ProductImage, long>>(() => new DapperProductImageRepository(connectionFactory));
        }

        // Repository Properties
        public IDapperRepository<UserProfile, long> UserProfiles => _userProfiles.Value;
        public IDapperRepository<Product, long> Products => _products.Value;
        public IDapperRepository<Order, long> Orders => _orders.Value;
        public IDapperRepository<OrderItem, long> OrderItems => _orderItems.Value;
        public IDapperRepository<Category, long> Categories => _categories.Value;
        public IDapperRepository<ProductAttribute, long> ProductAttributes => _productAttributes.Value;
        public IDapperRepository<ProductTemplate, long> ProductTemplates => _productTemplates.Value;
        public IDapperRepository<ProductImage, long> ProductImages => _productImages.Value;

        // Transaction Management
        public Task<IDbTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("Transaction already exists");

            var connection = _connectionFactory.CreateConnection();
            connection.Open();
            _currentTransaction = connection.BeginTransaction();
            return Task.FromResult(_currentTransaction);
        }

        public Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction");

            try
            {
                _currentTransaction.Commit();
            }
            catch
            {
                _currentTransaction.Rollback();
                throw;
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction.Connection?.Dispose();
                _currentTransaction = null;
            }

            return Task.CompletedTask;
        }

        public Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No active transaction");

            try
            {
                _currentTransaction.Rollback();
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction.Connection?.Dispose();
                _currentTransaction = null;
            }

            return Task.CompletedTask;
        }

        // Transaction Execution Helpers
        public async Task<T> ExecuteInTransactionAsync<T>(Func<IDbTransaction, Task<T>> operation)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                var result = await operation(transaction);
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<IDbTransaction, Task> operation)
        {
            using var transaction = await BeginTransactionAsync();
            try
            {
                await operation(transaction);
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        // Properties
        public bool HasActiveTransaction => _currentTransaction != null;
        public bool IsDisposed => _disposed;

        // Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                _currentTransaction?.Connection?.Dispose();
                _disposed = true;
            }
        }
    }

    // Concrete Dapper Repository implementations
    public class DapperUserProfileRepository : BaseDapperRepository<UserProfile, long>
    {
        public DapperUserProfileRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperProductRepository : BaseDapperRepository<Product, long>
    {
        public DapperProductRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperOrderRepository : BaseDapperRepository<Order, long>
    {
        public DapperOrderRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperOrderItemRepository : BaseDapperRepository<OrderItem, long>
    {
        public DapperOrderItemRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperCategoryRepository : BaseDapperRepository<Category, long>
    {
        public DapperCategoryRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperProductAttributeRepository : BaseDapperRepository<ProductAttribute, long>
    {
        public DapperProductAttributeRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperProductTemplateRepository : BaseDapperRepository<ProductTemplate, long>
    {
        public DapperProductTemplateRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }

    public class DapperProductImageRepository : BaseDapperRepository<ProductImage, long>
    {
        public DapperProductImageRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
} 