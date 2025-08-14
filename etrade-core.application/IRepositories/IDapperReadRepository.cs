using System.Data;

namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Dapper için projection-only read repository interface
    /// Performans odaklı, hafif okuma işlemleri için
    /// </summary>
    public interface IDapperReadRepository<TEntity, TKey> 
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Raw SQL ile sorgu çalıştırır
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        /// <summary>
        /// Raw SQL ile tek entity döner
        /// </summary>
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        /// <summary>
        /// Raw SQL ile scalar değer döner
        /// </summary>
        Task<T> QueryScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        /// <summary>
        /// Stored procedure çalıştırır
        /// </summary>
        Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null);
        
        /// <summary>
        /// Sayfalama ile sorgu çalıştırır
        /// </summary>
        Task<(IEnumerable<T> Items, int TotalCount)> QueryWithPaginationAsync<T>(
            string sql, 
            int pageNumber, 
            int pageSize, 
            object? param = null, 
            IDbTransaction? transaction = null, 
            int? commandTimeout = null);
        
        /// <summary>
        /// Multiple result set çalıştırır
        /// </summary>
        Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
    }
} 