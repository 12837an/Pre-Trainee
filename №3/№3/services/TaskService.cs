using TaskManager.models;
using TaskManager.repositories;

namespace TaskManager.services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllTasksAsync();
        }

        public async Task<int> AddTaskAsync(string title, string description)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            return await _taskRepository.AddTaskAsync(task);
        }

        public async Task<bool> UpdateTaskStatusAsync(int id, bool isCompleted)
        {
            return await _taskRepository.UpdateTaskStatusAsync(id, isCompleted);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            return await _taskRepository.DeleteTaskAsync(id);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }
    }
}