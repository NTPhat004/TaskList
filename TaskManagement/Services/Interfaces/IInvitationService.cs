using static TaskManagement.Models.GroupInvitationModel;
using TaskManagement.Models;

namespace TaskManagement.Services.Interfaces
{
    public interface IInvitationService
    {
        Task<GroupInvitationModel> InviteUserAsync(Guid inviterId, Guid groupId, string inviteeEmail);
        Task<List<GroupInvitationModel>> GetInvitationsForUserAsync(Guid userId);
        Task<bool> AcceptInvitationAsync(Guid invitationId);
        Task<bool> RejectInvitationAsync(Guid invitationId);
    }
}
