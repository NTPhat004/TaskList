using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface ITaskService
    {
        //Task
        Task<List<TaskModel>> GetTasksByUserIdAsync(Guid userId);
        Task<TaskModel> GetTaskByIdAsync(Guid taskId);
        Task<TaskModel> CreateTaskAsync(TaskModel task);
        Task DeleteTaskAsync(Guid taskId);
        Task UpdateTaskAsync(TaskModel task);

        //SubTask
        Task<List<SubTaskModel>> GetSubTasksByTaskIdAsync(Guid taskId);
        Task<SubTaskModel> GetSubTaskByIdAsync(Guid subTaskId);
        Task<SubTaskModel> CreateSubTaskAsync(SubTaskModel subTask);
        Task UpdateSubTaskAsync(SubTaskModel subTask);
        Task<SubTaskModel?> ToggleSubTaskStatusAsync(Guid subTaskId, Guid userId);
        Task<List<SubTaskModel>> GetTodaySubTasksAsync(Guid userId);

    }

}
