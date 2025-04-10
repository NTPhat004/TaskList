using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskModel>> GetTasksByUserIdAsync(Guid userId);
        Task<List<TaskModel>> GetTasksByGroupIdAsync(Guid groupId);
        Task<TaskModel> GetTaskByIdAsync(Guid taskId);
        Task<TaskModel> CreateAsync(TaskModel task);
        Task DeleteAsync(Guid taskId);
        Task UpdateTaskAsync(TaskModel task);



        Task<List<SubTaskModel>> GetSubTasksByTaskIdAsync(Guid taskId);
        Task<SubTaskModel> GetSubTaskByIdAsync(Guid subTaskId);
        Task<SubTaskModel> CreateAsync(SubTaskModel subTask);
        Task UpdateAsync(SubTaskModel subTask);
        Task<List<SubTaskModel>> GetTodaySubTasksAsync(Guid userId);
    }

}
