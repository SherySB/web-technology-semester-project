using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Daily_Task_Manager.Models.Data_Models;
using Daily_Task_Manager.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Daily_Task_Manager.Models.Services
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<UserTask>> GetAllTasksAsync()
        {
            using var conn = GetConnection();
            return await conn.QueryAsync<UserTask>("SELECT * FROM UserTasks");
        }

        public async Task<IEnumerable<UserTask>> GetTasksByUserAsync(string userId)
        {
            using var conn = GetConnection();
            return await conn.QueryAsync<UserTask>(
                "SELECT * FROM UserTasks WHERE UserId = @UserId",
                new { UserId = userId }
            );
        }

        public async Task<UserTask?> GetTaskByIdAsync(int id)
        {
            using var conn = GetConnection();
            return await conn.QueryFirstOrDefaultAsync<UserTask>(
                "SELECT * FROM UserTasks WHERE Id = @Id",
                new { Id = id });
        }

        public async Task AddTaskAsync(UserTask task)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                @"INSERT INTO UserTasks (Title, Description, IsCompleted, UserId)
                  VALUES (@Title, @Description, @IsCompleted, @UserId)",
                task
            );
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                @"UPDATE UserTasks 
                  SET Title = @Title,
                      Description = @Description,
                      IsCompleted = @IsCompleted
                  WHERE Id = @Id",
                task
            );
        }

        public async Task DeleteTaskAsync(int id)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                "DELETE FROM UserTasks WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
