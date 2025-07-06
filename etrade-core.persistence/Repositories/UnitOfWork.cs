using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace etrade_core.persistence.Repositories
{
    /// <summary>
    /// Unit of Work pattern implementation - Transaction yönetimi ve repository koordinasyonu
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DomainDbContext _context;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed = false;

        // Repository instances
        private IUserProfileRepository? _userProfileRepository;
        private IProductRepository? _productRepository;
        private IOrderRepository? _orderRepository;
        private IOrderItemRepository? _orderItemRepository;

        public UnitOfWork(DomainDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Repository properties with lazy loading
        public IUserProfileRepository UserProfiles => 
            _userProfileRepository ??= new UserProfileRepository(_context);

        public IProductRepository Products => 
            _productRepository ??= new ProductRepository(_context);

        public IOrderRepository Orders => 
            _orderRepository ??= new OrderRepository(_context);

        public IOrderItemRepository OrderItems => 
            _orderItemRepository ??= new OrderItemRepository(_context);

        // Transaction state
        public bool HasActiveTransaction => _currentTransaction != null;
        public bool IsDisposed => _disposed;

        /// <summary>
        /// Transaction başlatır
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (HasActiveTransaction)
            {
                throw new InvalidOperationException("Zaten aktif bir transaction bulunmaktadır.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Transaction'ı commit eder
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (!HasActiveTransaction)
            {
                throw new InvalidOperationException("Aktif transaction bulunmamaktadır.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction!.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Transaction'ı rollback eder
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (!HasActiveTransaction)
            {
                throw new InvalidOperationException("Aktif transaction bulunmamaktadır.");
            }

            try
            {
                await _currentTransaction!.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Değişiklikleri kaydeder
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Değişiklikleri kaydeder (cancellation token ile)
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Transaction ile birlikte çalıştırılacak işlemleri execute eder
        /// </summary>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            if (HasActiveTransaction)
            {
                // Eğer zaten transaction varsa, direkt işlemi çalıştır
                return await operation();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Transaction ile birlikte çalıştırılacak işlemleri execute eder (void)
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            if (HasActiveTransaction)
            {
                // Eğer zaten transaction varsa, direkt işlemi çalıştır
                await operation();
                return;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
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
                _context?.Dispose();
                _disposed = true;
            }
        }
    }
} 