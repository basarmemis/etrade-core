using System.Data;
using Dapper;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using etrade_core.application.Common.Base;
using etrade_core.domain.Entities.Base;

namespace etrade_core.persistence.Repositories.Base
{
    /// <summary>
    /// Dapper için read işlemlerini yapan base repository
    /// </summary>
    public abstract class BaseDapperReadRepository<TEntity, TKey> : IDapperReadRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly ITenantResolver? _tenantResolver;

        protected BaseDapperReadRepository(IDbConnectionFactory connectionFactory, ITenantResolver? tenantResolver = null)
        {
            _connectionFactory = connectionFactory;
            _tenantResolver = tenantResolver;
        }

        public virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(tenantParam.sql, tenantParam.param, transaction, commandTimeout, commandType);
        }

        public virtual async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(tenantParam.sql, tenantParam.param, transaction, commandTimeout, commandType);
        }

        public virtual async Task<T> QueryScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return (await connection.ExecuteScalarAsync<T>(tenantParam.sql, tenantParam.param, transaction, commandTimeout, commandType))!;
        }

        public virtual async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            var tenantParam = await AddTenantFilterAsync(storedProcedureName, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(tenantParam.sql, tenantParam.param, transaction, commandTimeout, CommandType.StoredProcedure);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> QueryWithPaginationAsync<T>(string sql, int pageNumber, int pageSize, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            
            // Add pagination parameters
            var paginationParam = new DynamicParameters(tenantParam.param);
            paginationParam.Add("@PageNumber", pageNumber);
            paginationParam.Add("@PageSize", pageSize);
            paginationParam.Add("@Offset", (pageNumber - 1) * pageSize);

            var items = await connection.QueryAsync<T>(tenantParam.sql, paginationParam, transaction, commandTimeout);
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM (" + tenantParam.sql + ") AS CountQuery", tenantParam.param, transaction, commandTimeout);

            return (items, totalCount);
        }

        public virtual async Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tenantParam = await AddTenantFilterAsync(sql, param);
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(tenantParam.sql, tenantParam.param, transaction, commandTimeout, commandType);
            
            var results = new List<IEnumerable<object>>();
            while (!multi.IsConsumed)
            {
                results.Add(await multi.ReadAsync<object>());
            }
            
            return results;
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
    }
} 