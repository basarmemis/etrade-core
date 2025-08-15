using System.Data;
using etrade_core.application.IRepositories;
using etrade_core.persistence.Context;
using etrade_core.application.Common.Base;

namespace etrade_core.persistence.Repositories.Base
{
    /// <summary>
    /// Dapper için hem read hem write işlemlerini birleştiren base repository
    /// Composition pattern kullanarak multiple inheritance sorununu çözer
    /// </summary>
    public abstract class BaseDapperRepository<TEntity, TKey> : IDapperRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        private readonly BaseDapperReadRepository<TEntity, TKey> _readRepository;
        private readonly BaseDapperWriteRepository<TEntity, TKey> _writeRepository;

        protected BaseDapperRepository(IDbConnectionFactory connectionFactory, ITenantResolver? tenantResolver = null)
        {
            _readRepository = new DapperReadRepositoryImpl<TEntity, TKey>(connectionFactory, tenantResolver);
            _writeRepository = new DapperWriteRepositoryImpl<TEntity, TKey>(connectionFactory, tenantResolver);
        }

        // IDapperReadRepository implementation
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => _readRepository.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => _readRepository.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<T> QueryScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => _readRepository.QueryScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);

        public Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _readRepository.QueryStoredProcedureAsync<T>(storedProcedureName, param, transaction, commandTimeout);

        public Task<(IEnumerable<T> Items, int TotalCount)> QueryWithPaginationAsync<T>(string sql, int pageNumber, int pageSize, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _readRepository.QueryWithPaginationAsync<T>(sql, pageNumber, pageSize, param, transaction, commandTimeout);

        public Task<IEnumerable<IEnumerable<object>>> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => _readRepository.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);

        // IDapperWriteRepository implementation
        public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
            => _writeRepository.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);

        public Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _writeRepository.ExecuteStoredProcedureAsync(storedProcedureName, param, transaction, commandTimeout);

        public Task<int> BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _writeRepository.BulkInsertAsync<T>(entities, tableName, transaction, commandTimeout);

        public Task<int> BulkUpdateAsync<T>(IEnumerable<T> entities, string tableName, string updateSql, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _writeRepository.BulkUpdateAsync<T>(entities, tableName, updateSql, transaction, commandTimeout);

        public Task<int> BulkDeleteAsync<T>(IEnumerable<TKey> ids, string tableName, IDbTransaction? transaction = null, int? commandTimeout = null)
            => _writeRepository.BulkDeleteAsync<T>(ids, tableName, transaction, commandTimeout);

        // Concrete implementations for composition
        private sealed class DapperReadRepositoryImpl<TEntityImpl, TKeyImpl> : BaseDapperReadRepository<TEntityImpl, TKeyImpl>
            where TEntityImpl : class
            where TKeyImpl : IEquatable<TKeyImpl>
        {
            public DapperReadRepositoryImpl(IDbConnectionFactory connectionFactory, ITenantResolver? tenantResolver) : base(connectionFactory, tenantResolver) { }
        }

        private sealed class DapperWriteRepositoryImpl<TEntityImpl, TKeyImpl> : BaseDapperWriteRepository<TEntityImpl, TKeyImpl>
            where TEntityImpl : class
            where TKeyImpl : IEquatable<TKeyImpl>
        {
            public DapperWriteRepositoryImpl(IDbConnectionFactory connectionFactory, ITenantResolver? tenantResolver) : base(connectionFactory, tenantResolver) { }
        }
    }
} 