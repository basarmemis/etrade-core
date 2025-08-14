using System.Data;

namespace etrade_core.persistence.Context
{
    /// <summary>
    /// Dapper için database connection factory interface
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Yeni bir database connection oluşturur
        /// </summary>
        IDbConnection CreateConnection();
        
        /// <summary>
        /// Connection string'i döner
        /// </summary>
        string GetConnectionString();
    }
} 