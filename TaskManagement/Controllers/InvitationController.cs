using Microsoft.AspNetCore.Mvc;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    public class InvitationController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IInvitationService _invitationService;

        public InvitationController(IAccountService accountService,IInvitationService invitationService)
        {
            _accountService = accountService;
            _invitationService = invitationService;
        }

        // Action để lấy tất cả lời mời của người dùng
        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var invitations = await _invitationService.GetInvitationsForUserAsync(userId); 
            return View(invitations);
        }

        // Action để chấp nhận lời mời và trả về PartialView
        [HttpPost]
        public async Task<IActionResult> AcceptInvitation(Guid invitationId)
        {
            var result = await _invitationService.AcceptInvitationAsync(invitationId);
            var userId = _accountService.GetCurrentUserId();
            var invitations = await _invitationService.GetInvitationsForUserAsync(userId);

            // Trả về PartialView với danh sách lời mời mới
            return PartialView("_InvitationList", invitations);
        }

        //Action để từ chối lời mời và trả về PartialView
       [HttpPost]
        public async Task<IActionResult> RejectInvitation(Guid invitationId)
        {
            var result = await _invitationService.RejectInvitationAsync(invitationId);
            var userId = _accountService.GetCurrentUserId();
            var invitations = await _invitationService.GetInvitationsForUserAsync(userId);

            // Trả về PartialView với danh sách lời mời mới
            return PartialView("_InvitationList", invitations);
        }
    }
}
