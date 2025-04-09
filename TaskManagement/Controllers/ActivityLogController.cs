using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Models;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    public class ActivityLogController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IAccountService accountService, IActivityLogService activityLogService)
        {
            _accountService = accountService;
            _activityLogService = activityLogService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var logs = await _activityLogService.GetLogsByUserAsync(userId);
            return View(logs); // Đảm bảo logs là List<ActivityLogModel>
        }

        [HttpGet]
        public async Task<IActionResult> GetLogsPartial(ActivityLogModel.ActivitySourceType? source)
        {
            var userId = _accountService.GetCurrentUserId();
            var logs = source.HasValue
                ? await _activityLogService.GetLogsBySourceAsync(userId, source.Value)
                : await _activityLogService.GetLogsByUserAsync(userId);

            return PartialView("_ActivityLogList", logs);
        }
    }
}
