namespace etrade_core.application.IRepositories
{
    /// <summary>
    /// Dapper için hem read hem write işlemlerini birleştiren repository interface
    /// EF Core ile birlikte kullanılabilir
    /// </summary>
    public interface IDapperRepository<TEntity, TKey> : 
        IDapperReadRepository<TEntity, TKey>, 
        IDapperWriteRepository<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
    }
} 