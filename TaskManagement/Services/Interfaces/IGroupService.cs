using TaskManagement.Common;
using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IGroupService
    {
        Task<ServiceResult<List<GroupModel>>> GetUserGroupsAsync(Guid userId);
        Task<ServiceResult<List<GroupModel>>> CreateGroupAsync(Guid userId, string groupName);
        Task<GroupModel?> GetGroupByIdAsync(Guid groupId);
        Task<List<TaskModel>> GetTasksByGroupIdAsync(Guid groupId);
    }
}
