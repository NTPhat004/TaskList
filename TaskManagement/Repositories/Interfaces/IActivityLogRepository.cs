using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<List<ActivityLogModel>> GetLogsByUserAsync(Guid userId);
        Task<List<ActivityLogModel>> GetLogsBySourceAsync(Guid userId, ActivityLogModel.ActivitySourceType source);
        Task AddAsync(ActivityLogModel log);
    }
}
