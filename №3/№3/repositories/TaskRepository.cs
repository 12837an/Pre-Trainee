using System.Data;
using Dapper;
using TaskManager.models;
using TaskManager.data;

namespace TaskManager.repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public TaskRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<TaskItem>("SELECT * FROM Tasks ORDER BY CreatedAt DESC");
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<TaskItem>(
                "SELECT * FROM Tasks WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> AddTaskAsync(TaskItem task)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Tasks (Title, Description, IsCompleted, CreatedAt) 
                       OUTPUT INSERTED.Id 
                       VALUES (@Title, @Description, @IsCompleted, @CreatedAt)";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                task.Title,
                task.Description,
                task.IsCompleted,
                task.CreatedAt
            });
        }
        public async Task<bool> UpdateTaskAsync(TaskItem task)  // ← Исправлено на TaskItem
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Tasks SET Title = @Title, Description = @Description, 
                       IsCompleted = @IsCompleted WHERE Id = @Id";

            var rowsChanged = await connection.ExecuteAsync(sql, task);
            return rowsChanged > 0;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rowsChanged = await connection.ExecuteAsync(
                "DELETE FROM Tasks WHERE Id = @Id", new { Id = id });

            return rowsChanged > 0;
        }

        public async Task<bool> UpdateTaskStatusAsync(int id, bool isCompleted)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rowsChanged = await connection.ExecuteAsync(
                "UPDATE Tasks SET IsCompleted = @IsCompleted WHERE Id = @Id",
             new { Id = id, IsCompleted = isCompleted });

            return rowsChanged > 0;
        }
    }
}