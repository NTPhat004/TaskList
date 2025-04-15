using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task AddMemberAsync(GroupInvitationModel groupInvitation);
        Task<List<GroupMemberModel>> GetGroupMembersByGroupIdAsync(Guid groupId);

        Task<List<UserModel>> GetGroupMembersAsync(Guid groupId);

    }
}
