using System.Data;
using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration;

namespace TaskManager.data
{
    public class SqlConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException("Строка подключения не найдена");
            }

            _connectionString = connString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
        public void Dispose()
        {
  
        }
    }
}