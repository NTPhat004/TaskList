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

        // Lấy danh sách hoạt động của User
        public async Task<List<ActivityLogModel>> GetLogsByUserAsync(Guid userId)
        {
            return await _logRepository.GetLogsByUserAsync(userId);
        }

        // Lấy danh sách hoạt động của User theo Mục
        public async Task<List<ActivityLogModel>> GetLogsBySourceAsync(Guid userId, ActivityLogModel.ActivitySourceType source)
        {
            return await _logRepository.GetLogsBySourceAsync(userId, source);
        }
        
        //Ghi lại hoạt động khi hoàn thành/hủy hoàn thành Task
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

        //Ghi lại hoạt động khi chấp nhận / từ chối lời mời 
        public async Task LogInvitationResponseAsync(Guid userId, Guid groupId, string groupName, string inviterEmail, string action)
        {
            var log = new ActivityLogModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Source = ActivityLogModel.ActivitySourceType.Inbox,
                Action = action, // "Chấp nhận lời mời" hoặc "Từ chối lời mời"
                Details = $"Bạn đã {action} tham gia nhóm \"{groupName}\" được mời bởi {inviterEmail}",
                Timestamp = DateTime.UtcNow,
                RelatedGroupId = groupId
            };

            await _logRepository.AddAsync(log);
        }

        //Ghi lại hoạt động  khi chấp nhận / từ chối lời mời cho người mời
        public async Task LogInvitationResultToInviterAsync(Guid inviterId, Guid groupId, string groupName, string inviteeEmail, string action)
        {
            var log = new ActivityLogModel
            {
                Id = Guid.NewGuid(),
                UserId = inviterId,
                Source = ActivityLogModel.ActivitySourceType.GroupTask,
                Action = action, // "Chấp nhận lời mời" hoặc "Từ chối lời mời"
                Details = $"{inviteeEmail} đã {action} tham gia nhóm \"{groupName}\" của bạn.",
                Timestamp = DateTime.UtcNow,
                RelatedGroupId = groupId
            };

            await _logRepository.AddAsync(log);
        }
    }
}
