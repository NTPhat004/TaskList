using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Models;
using TaskManagement.Models.ViewModels;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers
{
    [Route("task")]
    public class TaskController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITaskService _taskService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAssignmentService _assignmentService;

        public TaskController(ITaskService taskService, IAccountService accountService,IGroupMemberService groupMemberService, IAssignmentService assignmentService)
        {

            _taskService = taskService;
            _accountService = accountService;
            _groupMemberService = groupMemberService;
            _assignmentService = assignmentService;
        }

        //Danh sách công việc cá nhân
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var userId = _accountService.GetCurrentUserId();
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return View("Task",tasks);
        }

        //Thêm Task cá nhân
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

        //Thay đổi tên Task 
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

        //Check hoàn thành / bỏ hoàn thành Subtask
        [HttpPost("ToggleSubTaskStatus")]
        public async Task<IActionResult> ToggleSubTaskStatus(Guid subTaskId)
        {
            var userId = _accountService.GetCurrentUserId();
            var subTask = await _taskService.ToggleSubTaskStatusAsync(subTaskId,userId);
            if (subTask == null)
                return BadRequest(subTaskId);

            return PartialView("_SubTaskList", subTask); // ❗ Trả về content bên trong <li>
        }

        //Thêm GroupTask (Việc nhóm)
        [HttpPost("AddGroupTask")]
        public async Task<IActionResult> AddGroupTask(Guid groupId, string title)
        {
            var userId = _accountService.GetCurrentUserId();

            var newTask = new TaskModel
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                IsGroupTask = true,
                Title = title,
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _taskService.CreateTaskAsync(newTask);

            // Lấy lại danh sách Task của nhóm sau khi thêm
            var tasks = await _taskService.GetGroupTasksAsync(groupId);

            // Trả về PartialView danh sách task (dùng trong <select> của Modal SubTask)
            return PartialView("_TaskSelectPartial", tasks);
        }

        //Thêm GroupSubTask
        [HttpPost("AddGroupSubTask")]
        public async Task<IActionResult> AddGroupSubTask(Guid taskId, string title, DateTime? dueDate)
        {
            var userId = _accountService.GetCurrentUserId();

            var newSubTask = new SubTaskModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = title,
                DueDate = dueDate,
                IsCompleted = false,
                CreatedBy = userId,
            };

            await _taskService.CreateSubTaskAsync(newSubTask);

            // Load lại task để có thông tin SubTask + Assignments
            var task = await _taskService.GetTaskWithSubTasksAndAssignments(taskId);

            var createdSub = task.SubTasks.FirstOrDefault(x => x.Id == newSubTask.Id);

            return PartialView("_SubTaskRowPartial", new SubTaskViewModel
            {
                TaskTitle = task.Title,
                SubTask = createdSub
            });
        }

        //Hiển thị Nội dung SubTask để Edit.
        [HttpGet("GetTaskDetailModal")]
        public async Task<IActionResult> GetTaskDetailModal(Guid taskId,Guid groupId)
        {
            var subTask = await _taskService.GetSubTaskByIdAsync(taskId);
            var groupMembers =  await _groupMemberService.GetGroupMembersAsync(groupId);
            var task = await _taskService.GetGroupTasksAsync(groupId);

            var assignPopUp = new AssignPopupViewModel
            {
                GroupMembers = groupMembers,
                AssignedUserIds = subTask.Assignments.Select(a => a.UserId).ToList()
            };

            var viewModel = new TaskDetailModalViewModel
            {
                SubTask = subTask,
                Task = task,
                AssignPopupViewModel = assignPopUp
            };

            return PartialView("_TaskDetailModalPartial", viewModel);
        }

        //Lấy danh sách thành viên trong nhóm để hiển thị ở mục phân công
        [HttpGet("GetAssignPopup")]
        public async Task<IActionResult> LoadAssignPopup(Guid subTaskId)
        {
            var subTask = await _taskService.GetSubTaskDetailAsync(subTaskId);
            var groupMembers = await _groupMemberService.GetGroupMembersAsync(subTask.Task.GroupId.Value);

            var viewModel = new AssignPopupViewModel
            {
                GroupMembers = groupMembers,
                AssignedUserIds = subTask.Assignments.Select(a => a.UserId).ToList()
            };

            return PartialView("_AssignPopupPartial", viewModel);
        }

        //Xử lý phân công công việc
        [HttpPost("ToggleAssignment")]
        public async Task<IActionResult> ToggleAssignment(Guid subTaskId, Guid userId)
        {
            var isAssigned = await _assignmentService.ToggleAssignmentAsync(subTaskId, userId);
            var assignedUsers = await _assignmentService.GetAssignedUsersAsync(subTaskId);

            return Json(new
            {
                isAssigned,
                assignedUsers = assignedUsers.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    profilePicture = u.ProfilePicture
                })
            });
        }

        //Thay đổi tên SubTask 
        [HttpPost("UpdateSubTask")]
        public async Task<IActionResult> UpdateSubTask(Guid id, string title, string type)
        {
            var subTask = await _taskService.GetSubTaskByIdAsync(id);

            if (subTask == null) return NotFound("Không tìm thấy công việc.");

            switch (type)
            {
                case "title":
                    if (string.IsNullOrWhiteSpace(title)) return BadRequest("Tiêu đề không được để trống.");
                    subTask.Title = title;
                    break;

                case "parent":
                    if (!Guid.TryParse(title, out Guid parentTaskId)) return BadRequest("Mục không hợp lệ.");
                    subTask.TaskId = parentTaskId;
                    break;

                case "due":
                    if (!DateTime.TryParse(title, out DateTime dueDate)) return BadRequest("Ngày đến hạn không hợp lệ.");
                    subTask.DueDate = dueDate;
                    break;

                default:
                    return BadRequest("Loại cập nhật không hợp lệ.");
            }

            await _taskService.UpdateSubTaskAsync(subTask);
            return Ok();
        }


        //// POST: Lưu thông tin Task

    }
}
