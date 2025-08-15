using System.Data;
using Dapper;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using etrade_core.application.Common.Base;
using etrade_core.domain.Entities.Base;

namespace etrade_core.persistence.Repositories.Base
{
    /// <summary>
    /// Dapper için write işlemlerini yapan base repository
    /// </summary>
    public abstract class BaseDapperWriteRepository<TEntity, TKey> : IDapperWriteRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly ITenantResolver? _tenantResolver;

        protected BaseDapperWriteRepository(IDbConnectionFactory connectionFactory, ITenantResolver? tenantResolver = null)
        {
            _connectionFactory = connectionFactory;
            _tenantResolver = tenantResolver;
        }

        public virtual async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(tenantParam.sql, tenantParam.param, transaction, commandTimeout, commandType);
        }

        public virtual async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            var tenantParam = await AddTenantFilterAsync(storedProcedureName, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(tenantParam.sql, tenantParam.param, transaction, commandTimeout, CommandType.StoredProcedure);
        }

        public virtual async Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            if (_tenantResolver != null)
            {
                var currentTenantId = await _tenantResolver.ResolveTenantIdAsync();
                if (!string.IsNullOrEmpty(currentTenantId))
                {
                    // Add TenantId to all entities before bulk insert
                    foreach (var entity in entities)
                    {
                        if (entity is ITenantEntity tenantEntity && string.IsNullOrEmpty(tenantEntity.TenantId))
                        {
                            tenantEntity.TenantId = currentTenantId;
                        }
                    }
                }
            }

            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            var sql = $"INSERT INTO {tableName} ({GetColumnsForBulkInsert<T>()}) VALUES ({GetValuesForBulkInsert<T>()})";
            return await connection.ExecuteAsync(sql, entities, transaction, commandTimeout);
        }

        public virtual async Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities, string tableName, string updateSql, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            var tenantParam = await AddTenantFilterAsync(updateSql, null);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(tenantParam.sql, entities, transaction, commandTimeout);
        }

        public virtual async Task<int> BulkDeleteAsync<T>(IEnumerable<TKey> ids, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            var sql = $"DELETE FROM {tableName} WHERE Id IN @Ids";
            var tenantParam = await AddTenantFilterAsync(sql, new { Ids = ids });
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(tenantParam.sql, tenantParam.param, transaction, commandTimeout);
        }

        /// <summary>
        /// Adds tenant filter to SQL queries if tenant resolver is available
        /// </summary>
        /// <param name="sql">Original SQL query</param>
        /// <param name="param">Original parameters</param>
        /// <returns>Modified SQL and parameters with tenant filter</returns>
        protected virtual async Task<(string sql, object param)> AddTenantFilterAsync(string sql, object? param)
        {
            if (_tenantResolver == null)
                return (sql, param ?? new { });

            var currentTenantId = await _tenantResolver.ResolveTenantIdAsync();
            if (string.IsNullOrEmpty(currentTenantId))
                return (sql, param ?? new { });

            // Check if the SQL already contains tenant filtering
            if (sql.Contains("TenantId", StringComparison.OrdinalIgnoreCase))
                return (sql, param ?? new { });

            // Add tenant filter to WHERE clause
            var modifiedSql = AddTenantFilterToSql(sql);
            var modifiedParam = AddTenantFilterToParams(param, currentTenantId);

            return (modifiedSql, modifiedParam);
        }

        /// <summary>
        /// Adds tenant filter to SQL query
        /// </summary>
        /// <param name="sql">Original SQL</param>
        /// <returns>Modified SQL with tenant filter</returns>
        protected virtual string AddTenantFilterToSql(string sql)
        {
            // Simple approach: add WHERE clause if none exists, otherwise add AND
            if (sql.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            {
                return sql.Replace("WHERE", "WHERE TenantId = @TenantId AND", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return sql + " WHERE TenantId = @TenantId";
            }
        }

        /// <summary>
        /// Adds tenant parameter to existing parameters
        /// </summary>
        /// <param name="param">Original parameters</param>
        /// <param name="tenantId">Tenant ID to add</param>
        /// <returns>Modified parameters with tenant ID</returns>
        protected virtual object AddTenantFilterToParams(object? param, string tenantId)
        {
            if (param == null)
                return new { TenantId = tenantId };

            // Create new anonymous object with existing parameters plus TenantId
            var paramDict = new Dictionary<string, object>();
            
            // Extract properties from existing param object
            foreach (var prop in param.GetType().GetProperties())
            {
                paramDict[prop.Name] = prop.GetValue(param)!;
            }
            
            // Add TenantId
            paramDict["TenantId"] = tenantId;
            
            // Convert back to anonymous object (this is a simplified approach)
            return new DynamicParameters(paramDict);
        }

        /// <summary>
        /// Gets column names for bulk insert operations
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Comma-separated column names</returns>
        protected virtual string GetColumnsForBulkInsert<T>()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" || !p.PropertyType.IsValueType || p.PropertyType == typeof(int))
                .Select(p => p.Name);
            
            return string.Join(", ", properties);
        }

        /// <summary>
        /// Gets parameter placeholders for bulk insert operations
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Comma-separated parameter placeholders</returns>
        protected virtual string GetValuesForBulkInsert<T>()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" || !p.PropertyType.IsValueType || p.PropertyType == typeof(int))
                .Select(p => "@" + p.Name);
            
            return string.Join(", ", properties);
        }
    }
} 