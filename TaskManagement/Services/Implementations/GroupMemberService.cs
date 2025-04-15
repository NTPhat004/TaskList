using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;
using TaskManagement.Repositories.Implementations;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Services.Implementations
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupMemberRepository _groupMember;
        public GroupMemberService(IGroupMemberRepository groupMember)
        {
            _groupMember = groupMember;
        }
        public async Task AddMemberAsync(GroupInvitationModel groupInvitation)
        {
            var member = new GroupMemberModel
            {
                Id = Guid.NewGuid(),
                GroupId = groupInvitation.GroupId,
                UserId = groupInvitation.InviteeId.Value,
                Role = GroupMemberModel.GroupRole.Member, 
                JoinedAt = DateTime.UtcNow
            };
            await _groupMember.AddMemberAsync(member);
        }
        public async Task<List<GroupMemberModel>> GetGroupMembersByGroupIdAsync(Guid groupId)
        {
            return await _groupMember.GetGroupMembersByGroupIdAsync(groupId);
        }
        public async Task<List<UserModel>> GetGroupMembersAsync(Guid groupId)
        {
            return await _groupMember.GetMembersWithUserByGroupId(groupId);
        }
    }
}
