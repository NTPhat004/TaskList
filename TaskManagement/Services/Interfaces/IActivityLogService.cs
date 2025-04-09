using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IActivityLogService
    {
        Task<List<ActivityLogModel>> GetLogsByUserAsync(Guid userId);
        Task<List<ActivityLogModel>> GetLogsBySourceAsync(Guid userId, ActivityLogModel.ActivitySourceType source);
        Task LogSubTaskStatusChangeAsync(SubTaskModel subTask, Guid userId);
    }
}
