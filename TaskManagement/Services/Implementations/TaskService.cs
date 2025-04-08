using TaskManagement.Models;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskModel>> GetTasksByUserIdAsync(Guid userId)
        {
            return await _taskRepository.GetTasksByUserIdAsync(userId);
        }

        public async Task<TaskModel> GetTaskByIdAsync(Guid taskId)
        {
            return await _taskRepository.GetTaskByIdAsync(taskId);
        }

        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            return await _taskRepository.CreateAsync(task);
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            await _taskRepository.DeleteAsync(taskId);
        }

        public async Task UpdateTaskAsync(TaskModel task)
        {
            await _taskRepository.UpdateTaskAsync(task);
        }
        public async Task<List<SubTaskModel>> GetSubTasksByTaskIdAsync(Guid taskId)
        {
            return await _taskRepository.GetSubTasksByTaskIdAsync(taskId);
        }

        public async Task<SubTaskModel> GetSubTaskByIdAsync(Guid subTaskId)
        {
            return await _taskRepository.GetSubTaskByIdAsync(subTaskId);
        }

        public async Task<SubTaskModel> CreateSubTaskAsync(SubTaskModel subTask)
        {
            return await _taskRepository.CreateAsync(subTask);
        }

        public async Task UpdateSubTaskAsync(SubTaskModel subTask)
        {
            await _taskRepository.UpdateAsync(subTask);
        }
    }

}
