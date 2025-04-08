using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task AddMemberAsync(GroupInvitationModel groupInvitation);
    }
}
