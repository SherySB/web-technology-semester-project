using Daily_Task_Manager.Models.Data_Models;

using System.Collections.Generic;

namespace Daily_Task_Manager.Models.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<UserTask>> GetAllTasksAsync();
        Task<IEnumerable<UserTask>> GetTasksByUserAsync(string userId);
        Task<UserTask?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(UserTask task);
        Task UpdateTaskAsync(UserTask task);
        Task DeleteTaskAsync(int id);
    }
}

