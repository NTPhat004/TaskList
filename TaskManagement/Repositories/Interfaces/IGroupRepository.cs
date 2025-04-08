using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task AddGroupAsync(GroupModel group);
        Task<List<GroupModel>> GetUserGroupsAsync(Guid userId);
        Task<GroupModel?> GetByIdAsync(Guid groupId);
    }
}
