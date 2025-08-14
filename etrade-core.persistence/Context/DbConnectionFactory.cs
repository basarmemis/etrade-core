using System.Data;
using Npgsql;

namespace etrade_core.persistence.Context
{
    /// <summary>
    /// PostgreSQL için Dapper connection factory implementasyonu
    /// </summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            return connection;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
} 