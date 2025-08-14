using System.Data;

namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Dapper için write repository interface
    /// EF Core ile birlikte kullanılabilir
    /// </summary>
    public interface IDapperWriteRepository<TEntity, TKey> 
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Raw SQL ile insert, update, delete işlemleri
        /// </summary>
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        /// <summary>
        /// Stored procedure çalıştırır
        /// </summary>
        Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null);
        
        /// <summary>
        /// Bulk insert işlemi
        /// </summary>
        Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null);
        
        /// <summary>
        /// Bulk update işlemi
        /// </summary>
        Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities, string tableName, string updateSql, IDbTransaction? transaction = null, int? commandTimeout = null);
        
        /// <summary>
        /// Bulk delete işlemi
        /// </summary>
        Task<int> BulkDeleteAsync<T>(IEnumerable<TKey> ids, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null);
    }
} 