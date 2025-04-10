using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService _accountService;
        private readonly ITaskService _taskService;
        public HomeController(ILogger<HomeController> logger,IAccountService accountService, ITaskService taskService)
        {
            _logger = logger;
            _accountService = accountService;
            _taskService = taskService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var subTasks = await _taskService.GetTodaySubTasksAsync(userId);
            return View(subTasks);
        }


        public IActionResult Task()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
