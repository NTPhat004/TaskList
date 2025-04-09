using Microsoft.EntityFrameworkCore;
using TaskManagement.Common;
using TaskManagement.Models;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _logRepository;

        public ActivityLogService(IActivityLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<List<ActivityLogModel>> GetLogsByUserAsync(Guid userId)
        {
            return await _logRepository.GetLogsByUserAsync(userId);
        }

        public async Task<List<ActivityLogModel>> GetLogsBySourceAsync(Guid userId, ActivityLogModel.ActivitySourceType source)
        {
            return await _logRepository.GetLogsBySourceAsync(userId, source);
        }

        public async Task LogSubTaskStatusChangeAsync(SubTaskModel subTask, Guid userId)
        {
            var log = new ActivityLogModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = subTask.IsCompleted ? "Đánh dấu hoàn thành SubTask" : "Bỏ đánh dấu hoàn thành SubTask",
                Source = ActivityLogModel.ActivitySourceType.PersonalTask,
                Details = $"SubTask: {subTask.Title}",
                Timestamp = DateTime.UtcNow,
                RelatedSubTaskId = subTask.Id,
                RelatedTaskId = subTask.TaskId // nếu có thuộc tính này trong SubTaskModel
            };

            await _logRepository.AddAsync(log);
        }
    }
}
