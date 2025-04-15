using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IGroupMemberRepository
    {
       Task AddMemberAsync(GroupMemberModel member);
        Task<List<GroupMemberModel>> GetGroupMembersByGroupIdAsync(Guid groupId);
        Task<List<UserModel>> GetMembersWithUserByGroupId(Guid groupId);
    }
}
