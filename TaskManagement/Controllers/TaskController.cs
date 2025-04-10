using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Models;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    [Route("task")]
    public class TaskController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService, IAccountService accountService)
        {

            _taskService = taskService;
            _accountService = accountService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return View("Task",tasks);
        }

        [HttpPost("AddTask")]
        public async Task<IActionResult> AddTask()
        {
            var userId = _accountService.GetCurrentUserId();
            var newTask = new TaskModel
            {
                Id = Guid.NewGuid(),
                IsGroupTask = false,
                Title = "Công việc mới",
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _taskService.CreateTaskAsync(newTask);

            return PartialView("_TaskItem", newTask); // Trả về PartialView để render vào UI
        }

        [HttpPost("UpdateTitle")]
        public async Task<IActionResult> UpdateTitle(Guid id, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Tiêu đề không được để trống.");
            }

            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound("Không tìm thấy công việc.");
            }

            task.Title = title;
            await _taskService.UpdateTaskAsync(task);

            return Ok();
        }


        //Thêm SubTask vào Task cụ thể (dùng AJAX)
        [HttpPost("AddSubTask")]
        public async Task<IActionResult> AddSubTask(Guid taskId, string name, DateTime? deadline)
        {
            var userId = _accountService.GetCurrentUserId();
            var newSubTask = new SubTaskModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title =  name,
                DueDate = deadline,
                IsCompleted = false,
                CreatedBy = userId
            };

            await _taskService.CreateSubTaskAsync(newSubTask);

            return PartialView("_SubTaskItem", newSubTask); // Trả về Partial View của SubTask
        }

        [HttpPost("update-subtask")]
        public async Task<IActionResult> UpdateSubTaskTask(Guid subTaskId, Guid newTaskId)
        {
            var subTask = await _taskService.GetSubTaskByIdAsync(subTaskId);
            if (subTask == null) return NotFound("SubTask không tồn tại.");

            subTask.TaskId = newTaskId;
            await _taskService.UpdateSubTaskAsync(subTask);

            return Ok(new { success = true, message = "SubTask đã được cập nhật!" });
        }

        [HttpPost("ToggleSubTaskStatus")]
        public async Task<IActionResult> ToggleSubTaskStatus(Guid subTaskId)
        {
            var userId = _accountService.GetCurrentUserId();
            var subTask = await _taskService.ToggleSubTaskStatusAsync(subTaskId,userId);
            if (subTask == null)
                return BadRequest(subTaskId);

            return PartialView("_SubTaskList", subTask); // ❗ Trả về content bên trong <li>
        }


        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
