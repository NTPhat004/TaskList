using static TaskManagement.Models.GroupInvitationModel;
using TaskManagement.Models;
using TaskManagement.Repositories.Interfaces;
using TaskManagement.Services.Interfaces;
using TaskManagement.Repositories.Implementations;

namespace TaskManagement.Services.Implementations
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IGroupMemberService _groupMemberService;
        public InvitationService(IInvitationRepository repo, IUserRepository userRepo, IGroupMemberService groupMemberService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _groupMemberService = groupMemberService;
        }

        //Tạo lời mời.
        public async Task<GroupInvitationModel> InviteUserAsync(Guid inviterId, Guid groupId, string inviteeEmail)
        {
            var user = await _userRepo.GetUserByEmailAsync(inviteeEmail);
            var invitation = new GroupInvitationModel
            {
                GroupId = groupId,
                InviterId = inviterId,
                InviteeEmail = inviteeEmail,
                InviteeId = user?.Id
            };

            return await _repo.CreateInvitationAsync(invitation);
        }

        //Lấy danh sách lời mời của User.
        public Task<List<GroupInvitationModel>> GetInvitationsForUserAsync(Guid userId)
            => _repo.GetUserInvitationsAsync(userId);

        //Chấp nhận lời mời.
        public async Task<bool> AcceptInvitationAsync(Guid invitationId)
        {
            var invitation = await _repo.GetInvitationByIdAsync(invitationId);

            if (invitation != null && invitation.Status == InvitationStatus.Pending)
            {
                invitation.Status = InvitationStatus.Accepted;
                await _repo.UpdateInvitationStatusAsync(invitation);

                // Nếu có InviteeId thì thêm vào GroupMember
                if (invitation.InviteeId.HasValue)
                {
                    await _groupMemberService.AddMemberAsync(invitation);
                }

                return true;
            }
            return false;
        }
        //Từ chối lời mời
        public async Task<bool> RejectInvitationAsync(Guid invitationId)
        {
            var invitation = await _repo.GetInvitationByIdAsync(invitationId);

            if (invitation != null && invitation.Status == InvitationStatus.Pending)
            {
                invitation.Status = InvitationStatus.Rejected;
                await _repo.UpdateInvitationStatusAsync(invitation);
                return true;
            }

            return false;
        }

    }

}
