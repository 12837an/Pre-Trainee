using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TaskManager.data
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync();
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IConfiguration _configuration;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            await CreateDatabaseIfNotExistsAsync();
            await CreateTableIfNotExistsAsync();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            var masterConnectionString = _configuration.GetConnectionString("DefaultConnection")
                .Replace("Database=TaskManager", "Database=master");

            using var masterConnection = new SqlConnection(masterConnectionString);

            var createDbSql = @"
                IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TaskManager')
                BEGIN
                    CREATE DATABASE TaskManager;
                END";

            await masterConnection.ExecuteAsync(createDbSql);
        }

        private async Task CreateTableIfNotExistsAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var createTableSql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
                CREATE TABLE Tasks (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Title NVARCHAR(255) NOT NULL,
                    Description NVARCHAR(MAX),
                    IsCompleted BIT NOT NULL DEFAULT 0,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                )";

            await connection.ExecuteAsync(createTableSql);
        }
    }
}