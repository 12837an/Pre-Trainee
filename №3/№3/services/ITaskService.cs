using TaskManager.models;

namespace TaskManager.services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<int> AddTaskAsync(string title, string description);
        Task<bool> UpdateTaskStatusAsync(int id, bool isCompleted);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskItem?> GetTaskByIdAsync(int id);
    }
}