using System.Data;
using Dapper;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;

namespace etrade_core.persistence.Repositories.Base
{
    /// <summary>
    /// Dapper için base read repository implementasyonu
    /// </summary>
    public abstract class BaseDapperReadRepository<TEntity, TKey> : IDapperReadRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseDapperReadRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public virtual async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            var result = await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
            return result;
        }

        public virtual async Task<T> QueryScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            var result = await connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
            if (result == null)
                throw new InvalidOperationException("Query returned null result");
            return result;
        }

        public virtual async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            return await connection.QueryAsync<T>(storedProcedureName, param, transaction, commandTimeout, CommandType.StoredProcedure);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> QueryWithPaginationAsync<T>(
            string sql, 
            int pageNumber, 
            int pageSize, 
            object? param = null, 
            IDbTransaction? transaction = null, 
            int? commandTimeout = null)
        {
            // Count query için SQL'i modify et
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            
            // Pagination için SQL'e OFFSET ve LIMIT ekle
            var paginatedSql = $"{sql} OFFSET {(pageNumber - 1) * pageSize} LIMIT {pageSize}";
            
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, param, transaction, commandTimeout);
            var items = await connection.QueryAsync<T>(paginatedSql, param, transaction, commandTimeout);
            
            return (items, totalCount);
        }

        public virtual async Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            using var multi = await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
            var results = new List<IEnumerable<object>>();
            
            while (!multi.IsConsumed)
            {
                results.Add(multi.Read());
            }
            
            return results;
        }
    }
} 