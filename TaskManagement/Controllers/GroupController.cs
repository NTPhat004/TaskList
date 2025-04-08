using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TaskManagement.Common;
using TaskManagement.Hubs;
using TaskManagement.Models;
using TaskManagement.Models.ViewModels;
using TaskManagement.Services;
using TaskManagement.Services.Implementations;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    public class GroupController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IAccountService _accountService;
        private readonly IGroupService _groupService;
        private readonly IInvitationService _invitationService;

        public GroupController(IHubContext<NotificationHub> hubContext, IAccountService accountService, IGroupService groupService, IInvitationService invitationService)
        {
            _hubContext = hubContext;
            _accountService = accountService;
            _groupService = groupService;
            _invitationService = invitationService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var result = await _groupService.GetUserGroupsAsync(userId);

            if (result.Status == ServiceResultStatus.NotFound)
            {
                ViewBag.Message = result.Message;
                return View(new List<GroupModel>()); // Trả về danh sách rỗng
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string groupName)
        {
            var userId = _accountService.GetCurrentUserId();
            var result = await _groupService.CreateGroupAsync(userId, groupName);

            if (result.Status != ServiceResultStatus.Success)
            {
                return BadRequest(result.Message);
            }

            return PartialView("_GroupListPartial", result.Data); // Render lại danh sách Group
        }

        // GET: /Group/Tasks/{groupId}
        public async Task<IActionResult> Tasks(Guid groupId)
        {
            var group = await _groupService.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return NotFound();
            }

            var tasks = await _groupService.GetTasksByGroupIdAsync(groupId);

            var viewModel = new GroupTasksViewModel
            {
                Group = group,
                Tasks = tasks
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> InviteMember(Guid groupId, string email)
        {
            var inviterId = _accountService.GetCurrentUserId(); // hàm lấy từ User.Identity

            var invitation = await _invitationService.InviteUserAsync(inviterId, groupId, email);

            if (invitation.InviteeId != null)
            {
                await _hubContext.Clients.User(invitation.InviteeId.ToString())
                .SendAsync("ReceiveInvitation", $"Bạn có lời mời tham gia nhóm: {invitation.Group.Name}");
            }

            return Ok();
        }
    }
}
