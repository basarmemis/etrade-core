using System.Data;
using Dapper;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;

namespace etrade_core.persistence.Repositories.Base
{
    /// <summary>
    /// Dapper için base write repository implementasyonu
    /// </summary>
    public abstract class BaseDapperWriteRepository<TEntity, TKey> : IDapperWriteRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseDapperWriteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public virtual async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public virtual async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            return await connection.ExecuteAsync(storedProcedureName, param, transaction, commandTimeout, CommandType.StoredProcedure);
        }

        public virtual async Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            if (!entities.Any()) return 0;
            
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            // PostgreSQL için bulk insert
            var columns = GetColumnsFromType<T>();
            var values = string.Join(", ", entities.Select((_, index) => $"({string.Join(", ", columns.Select(c => $"@{c}{index}"))})"));
            
            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES {values}";
            
            var parameters = new DynamicParameters();
            var entityIndex = 0;
            foreach (var entity in entities)
            {
                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity);
                    parameters.Add($"{prop.Name}{entityIndex}", value);
                }
                entityIndex++;
            }
            
            return await connection.ExecuteAsync(sql, parameters, transaction, commandTimeout);
        }

        public virtual async Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities, string tableName, string updateSql, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            if (!entities.Any()) return 0;
            
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            var totalAffected = 0;
            foreach (var entity in entities)
            {
                var affected = await connection.ExecuteAsync(updateSql, entity, transaction, commandTimeout);
                totalAffected += affected;
            }
            
            return totalAffected;
        }

        public virtual async Task<int> BulkDeleteAsync<T>(IEnumerable<TKey> ids, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            if (!ids.Any()) return 0;
            
            using var connection = _connectionFactory.CreateConnection();
            if (transaction != null)
                connection.ConnectionString = transaction.Connection?.ConnectionString;
                
            var idList = string.Join(", ", ids.Select((_, index) => $"@id{index}"));
            var sql = $"DELETE FROM {tableName} WHERE Id IN ({idList})";
            
            var parameters = new DynamicParameters();
            var idIndex = 0;
            foreach (var id in ids)
            {
                parameters.Add($"id{idIndex}", id);
                idIndex++;
            }
            
            return await connection.ExecuteAsync(sql, parameters, transaction, commandTimeout);
        }

        private static string[] GetColumnsFromType<T>()
        {
            return typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p => p.Name)
                .ToArray();
        }
    }
} 