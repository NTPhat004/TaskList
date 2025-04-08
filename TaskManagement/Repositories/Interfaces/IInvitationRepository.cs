using static TaskManagement.Models.GroupInvitationModel;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Interfaces
{
    public interface IInvitationRepository
    {
        Task<GroupInvitationModel> CreateInvitationAsync(GroupInvitationModel invitation);
        Task<List<GroupInvitationModel>> GetUserInvitationsAsync(Guid userId);
        Task UpdateInvitationStatusAsync(GroupInvitationModel invite);
        Task<GroupInvitationModel?> GetInvitationByIdAsync(Guid invitationId);
    }
}
